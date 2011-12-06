using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using AutoMapper;
using NHibernate;
using NLog;
using Remotion.Linq.Collections;
using Status.Etl.Csv;
using Status.Model;
using Status.Repository;

namespace Status.ETL.Csv
{
    public class CsvStatusEtlBridge : ICsvStatusEtlBridge
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IStatusReportRepository StatusReportRepository { get; set; }

        public IProjectRepository ProjectRepository { get; set; }

        public ITopicRepository TopicRepository { get; set; }

        public IResourceRepository ResourceRepository { get; set; }

        public ITeamRepository TeamRepository { get; set; }

        public CsvStatusEtlBridge(IStatusReportRepository statusReportRepository, IProjectRepository projectRepository, ITopicRepository topicRepository, IResourceRepository resourceRepository, ITeamRepository teamRepository)
        {
            StatusReportRepository = statusReportRepository;
            ProjectRepository = projectRepository;
            TopicRepository = topicRepository;
            ResourceRepository = resourceRepository;
            TeamRepository = teamRepository;
        }

        public void UpsertStatus(IList<StatusCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);

            // we'll be sharing a single unitofwork for this operation
            ITransaction transaction = this.StatusReportRepository.BeginTransaction();

            // share the session - ugly
            this.ProjectRepository.Session = this.TopicRepository.Session = this.StatusReportRepository.Session;
            try
            {
                // create all resources
                // ensure all resources are in place
                // for now, we put a stub resource in this
                var grpResources = items.GroupBy(item => item.TeamLead);
                foreach (var resourceG in grpResources)
                {
                    var resources = this.ResourceRepository.GetResourcesByName(resourceG.Key);
                    var res = resourceG.ToList().First();
                    if (resources.Count == 0)
                        this.ResourceRepository.AddResource(new Employee() {FullName = res.TeamLead, EmailAddress = String.Format("{0}@test.com", resourceG.Key.Replace(" ", ".")) });
                }

                var windowsIdentity = WindowsIdentity.GetCurrent();
                Resource dummyResource = null;
                if (windowsIdentity != null)
                {
                    var login = windowsIdentity.Name;
                    dummyResource = new Employee()
                                        {
                                            WindowsLogin = login,
                                            EmailAddress = String.Format("{0}@test.com", login),
                                            FullName = "First Last"
                                        };
                } else
                    dummyResource = new Resource() { EmailAddress = "t@t.com", FirstName = "FN", LastName = "LN" };
                this.ResourceRepository.AddResource(dummyResource);

                // create all teams
                var grpTeams = items.GroupBy(item => item.TeamName);
                foreach (var teamG in grpTeams)
                {
                    var team = this.TeamRepository.GetTeamByName(teamG.Key);
                    if (team == null)
                    {
                        // pull the details of team name / lead from first item
                        // lookup lead by eamil address?
                        var teamCsv = teamG.ToList()[0];
                        var lead = this.ResourceRepository.GetResourcesByName(teamCsv.TeamLead).SingleOrDefault();
                        var t = new Team() { Name = teamG.Key, Lead = (Employee)lead };
                        this.TeamRepository.AddTeam(t);
                    }
                }

                // things to check
                // all projects exist and project id's returned
                var grpProjects = items.GroupBy(item => item.Project);

                IList<string> projectNames = grpProjects.Select(g => g.Key).ToList();

                // all should be encapsulated in repository later on...
                // below is sql retrieval optimization but makes updating more complicated
                var projects = this.ProjectRepository.GetProjectsByNames(projectNames);

                foreach (var project in projects)
                {
                    _logger.Debug("Project found: {0}", project.Name);
                    projectNames.Remove(project.Name);
                }

                // any remaining projects will need to be created before moving forward
                // we can derive team from project team on any status item in the list
                projectNames.ToList().ForEach(p => this.ProjectRepository.AddProject(new Project() { Name = p, Caption = p }));

                // map JIRA ID's to topics in the new system
                var grpTopics = items.GroupBy(item => item.JiraID);

                grpTopics.ToList().ForEach(
                    gt =>
                    {
                        var jiraId = gt.Key;
                        // empty strings are inevitable, create new topics for them
                        Topic t = null;
                        if (jiraId != string.Empty)
                        {
                            t = this.TopicRepository.GetTopicByExternalId(jiraId);
                            if (t == null)
                            {
                                // steal the topic from the first item in list
                                var firstItem = gt.First();
                                this.TopicRepository.AddTopic(new JiraIssueTopic() { JiraId = jiraId, Caption = firstItem.Note });
                            }
                        }
                        else
                        {
                            // all the empty topics will be grouped together, so if that is the case, create as new topics now
                            gt.ToList().ForEach(item => this.TopicRepository.AddTopic(new Topic() { Caption = item.Note }));
                        }

                    });

                // get all status dates to see if we are overwriting an existing report, if so, delete the old one
                var grpStatusDates = items.GroupBy(item => item.StatusDate);

                // the following should be done in a transaction.
                grpStatusDates.ToList().ForEach(
                    gsd =>
                    {
                        var statusReportDate = gsd.Key;
                        try { this.StatusReportRepository.DeleteStatusReport(statusReportDate); }
                        catch (NullReferenceException) { }
                        var statusReport = StatusReport.Create(statusReportDate, String.Format("Status report for {0:MM/dd/yyyy}", statusReportDate));
                        // iterate through each item for that date and add them
                        gsd.ToList().ForEach(statusReportItem =>
                                                 {
                                                     // we need to construct statusitem and topic for this
                                                     var statusItem = new StatusItem();
                                                     // we could use AutoMapper here - but doing manually for now as custom logic abound
                                                     statusItem.Topic = this.TopicRepository.GetTopicByExternalId(statusReportItem.JiraID);
                                                     if (statusItem.Topic == null)
                                                         statusItem.Topic = this.TopicRepository.GetTopicByCaption(statusReportItem.Note);

                                                     statusItem.Project = this.ProjectRepository.GetProject(statusReportItem.Project);

                                                     statusItem.Milestone = new Milestone()
                                                     {
                                                         ConfidenceLevel = statusReportItem.MilestoneConfidence ?? MilestoneConfidenceLevels.High,
                                                         Date = statusReportItem.MilestoneDate,
                                                         Type = statusReportItem.StatusType
                                                     };
                                                     statusItem.Caption = statusReportItem.Caption;
                                                     statusItem.Notes.Add(new Note()
                                                                              {
                                                                                  AuditInfo = new AuditInfo(dummyResource),
                                                                                  Text = statusReportItem.Note
                                                                              });
                                                     if (statusItem.Topic != null)
                                                         statusReport.AddStatusItem(statusItem);
                                                     else
                                                         _logger.Warn("Skilling statusitem {0} as no topic assigned", statusItem.Caption);
                                                 });
                        this.StatusReportRepository.AddStatusReport(statusReport);
                    });
                // import the new status report items

                this.StatusReportRepository.CommitTransaction();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Unable to import from CsvStatus file", exc);
                this.StatusReportRepository.RollbackTransaction();
                throw;
            }
            finally
            {
                // this.StatusReportRepository.RollbackTransaction();
            }
        }
    }
}

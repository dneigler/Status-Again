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

        public IDepartmentRepository DepartmentRepository { get; set; }

        public CsvStatusEtlBridge(IStatusReportRepository statusReportRepository, IProjectRepository projectRepository, ITopicRepository topicRepository, IResourceRepository resourceRepository, ITeamRepository teamRepository, IDepartmentRepository departmentRepository)
        {
            StatusReportRepository = statusReportRepository;
            ProjectRepository = projectRepository;
            TopicRepository = topicRepository;
            ResourceRepository = resourceRepository;
            TeamRepository = teamRepository;
            DepartmentRepository = departmentRepository;
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

                // create dummy department
                var department = this.DepartmentRepository.GetByName("Department");
                if (department == null)
                {
                    department = new Department() { Name = "Department", Manager = dummyResource as Employee };
                    this.DepartmentRepository.Add(department);
                }

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

                        var lead = this.ResourceRepository.GetResourcesByName(teamCsv.TeamLead).SingleOrDefault() as Employee;
                        var t = new Team() { Name = teamG.Key, Lead = lead, Department = department};
                        t.Members.Add(lead);
                        t = this.TeamRepository.Add(t);
                        lead.Team = t;
                        this.ResourceRepository.Update(lead);
                    }
                }

                // things to check
                // all projects exist and project id's returned
                var grpProjects = items.GroupBy(item => item.Project);
                foreach (var projectG in grpProjects)
                {
                    var p = this.ProjectRepository.GetProjectByName(projectG.Key);
                    if (p == null)
                    {
                        var pItem = projectG.First();
                        var pTeam = this.TeamRepository.GetTeamByName(pItem.TeamName); // not going to work with this file format as team id is made up
                        var pLead = this.ResourceRepository.GetResourcesByName(pItem.TeamLead)[0];
                        var project = new Project() { 
                            Name = pItem.Project, 
                            Caption = pItem.ProjectSummary, 
                            Lead=pLead as Employee,
                            Department = department,
                            Team = pTeam
                        };
                        this.ProjectRepository.AddProject(project);
                    }
                }

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
                                this.TopicRepository.Add(new JiraIssueTopic() { JiraId = jiraId, Caption = firstItem.Note });
                            }
                        }
                        else
                        {
                            // all the empty topics will be grouped together, so if that is the case, create as new topics now
                            gt.ToList().ForEach(item => this.TopicRepository.Add(new Topic() { Caption = item.Note }));
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
                        statusReport.AuditInfo = new AuditInfo(dummyResource);
                        // iterate through each item for that date and add them
                        gsd.ToList().ForEach(statusReportItem =>
                                                 {
                                                     // we need to construct statusitem and topic for this
                                                     var statusItem = new StatusItem();
                                                     // we could use AutoMapper here - but doing manually for now as custom logic abound
                                                     statusItem.Topic = this.TopicRepository.GetTopicByExternalId(statusReportItem.JiraID);
                                                     if (statusItem.Topic == null)
                                                         statusItem.Topic = this.TopicRepository.GetTopicByCaption(statusReportItem.Note);

                                                     statusItem.Project = this.ProjectRepository.GetProjectByName(statusReportItem.Project);
                                                     statusItem.AuditInfo = new AuditInfo(dummyResource);
                                                     statusItem.Milestone = new Milestone()
                                                     {
                                                         ConfidenceLevel = statusReportItem.MilestoneConfidence ?? MilestoneConfidenceLevels.High,
                                                         Date = statusReportItem.MilestoneDate,
                                                         Type = statusReportItem.StatusType
                                                     };
                                                     statusItem.Caption = statusReportItem.Note; //.Caption;
                                                     statusItem.Notes.Add(new Note()
                                                                              {
                                                                                  AuditInfo = new AuditInfo(dummyResource),
                                                                                  Text = statusReportItem.Note
                                                                              });
                                                     if (statusItem.Topic != null)
                                                     {
                                                         statusReport.AddStatusItem(statusItem);
                                                         statusItem.StatusReport = statusReport;
                                                     } 
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
            }
        }
    }
}

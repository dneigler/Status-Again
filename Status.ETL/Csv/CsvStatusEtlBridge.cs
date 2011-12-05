using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using NHibernate;
using NLog;
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

        public CsvStatusEtlBridge(IStatusReportRepository statusReportRepository, IProjectRepository projectRepository, ITopicRepository topicRepository)
        {
            StatusReportRepository = statusReportRepository;
            ProjectRepository = projectRepository;
            TopicRepository = topicRepository;

            // Mapper.CreateMap<StatusCsvItem, StatusItem>();
        }

        public void UpsertStatus(IList<Etl.Csv.StatusCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);

            // we'll be sharing a single unitofwork for this operation
            ITransaction transaction = this.StatusReportRepository.BeginTransaction();
            
            // share the session - ugly
            this.ProjectRepository.Session = this.TopicRepository.Session = this.StatusReportRepository.Session;
            try
            {
                
                // things to check
                // all projects exist and project id's returned
                var grpProjects = items.GroupBy(item => item.Project); //.OrderBy(i => i.ToString());

                IList<string> projectNames = grpProjects.Select(g => g.Key).ToList();
                // below is sql retrieval optimization but makes updating more complicated
                var projects = this.ProjectRepository.GetProjectsByNames(projectNames);

                foreach (var project in projects)
                {
                    _logger.Debug("Project found: {0}", project.Name);
                    projectNames.Remove(project.Name);
                }

                // any remaining projects will need to be created before moving forward
                projectNames.ToList().ForEach(p => this.ProjectRepository.AddProject(new Project() {Name = p}));

                // map JIRA ID's to topics in the new system
                var grpTopics = items.GroupBy(item => item.JiraID);

                grpTopics.ToList().ForEach(
                    gt =>
                    {
                        var jiraId = gt.Key;
                        // empty strings are inevitable, create new topics for them
                        Topic t = null;
                        if (jiraId != string.Empty)
                            t = this.TopicRepository.GetTopicByExternalId(jiraId);
                        if (t == null)
                        {
                            // steal the topic from the first item in list
                            var firstItem = gt.First();
                            t = (jiraId == string.Empty ? 
                                new Topic() { Caption = firstItem.Note } : 
                                new JiraIssueTopic() {JiraId = jiraId, Caption = firstItem.Note});
                            // TODO: add logic for fix version lookup later
                            this.TopicRepository.AddTopic(t);
                        }
                    });

                // get all status dates to see if we are overwriting an existing report, if so, delete the old one
                var grpStatusDates = items.GroupBy(item => item.StatusDate);

                // the following should be done in a transaction.
                grpStatusDates.ToList().ForEach(
                    gsd =>
                        {
                            var statusReportDate = gsd.Key;
                            try {this.StatusReportRepository.DeleteStatusReport(statusReportDate);} catch (NullReferenceException){}
                            var statusReport = StatusReport.Create(statusReportDate, String.Format("Status report for {0:MM/dd/yyyy}", statusReportDate));
                            // iterate through each item for that date and add them
                            gsd.ToList().ForEach(statusReportItem =>
                                                     {
                                                         // we need to construct statusitem and topic for this
                                                         var statusItem = new StatusItem();
                                                         // we could use AutoMapper here - but doing manually for now as custom logic abound
                                                         statusItem.Topic = this.TopicRepository.GetTopicByExternalId(statusReportItem.JiraID);
                                                         if (statusItem.Topic == null)
                                                             statusItem.Topic =
                                                                 this.TopicRepository.GetTopicByCaption(
                                                                     statusReportItem.Note);
                                                         statusItem.Milestone = new Milestone() {
                                                             ConfidenceLevel = statusReportItem.MilestoneConfidence ?? MilestoneConfidenceLevels.High,
                                                         Date=statusReportItem.MilestoneDate,
                                                         Type = statusReportItem.StatusType};
                                                         statusItem.Caption = statusReportItem.Caption;
                                                         statusItem.Notes.Add(new Note()
                                                                                  {
                                                                                      AuditInfo = new AuditInfo(new Resource(){EmailAddress = "t@t.com", FirstName = "FN", LastName = "LN"}),
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

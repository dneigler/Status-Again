using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
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
        }

        public void UpsertStatus(IList<Etl.Csv.StatusCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);
            // things to check
            // all projects exist and project id's returned
            var grpProjects = items.GroupBy(item => item.Project);//.OrderBy(i => i.ToString());

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
            IList<string> jiraIds = grpTopics.Select(g => g.Key).ToList();
            
            jiraIds.ToList().ForEach(j =>
                                         {
                                             var t = this.TopicRepository.GetTopicByExternalId(j) ??
                                                       new JiraIssueTopic() { JiraId = j };
                                         });

            // get all status dates to see if we are overwriting an existing report, if so, delete the old one
            var grpStatusDates = items.GroupBy(item => item.StatusDate);

            // items.ToList().ForEach(item => item.
        }
    }
}

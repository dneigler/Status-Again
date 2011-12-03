using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Status.Repository;

namespace Status.ETL.Csv
{
    public class CsvStatusEtlBridge : ICsvStatusEtlBridge
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IStatusReportRepository StatusReportRepository { get; set; }

        public IProjectRepository ProjectRepository { get; set; }

        public CsvStatusEtlBridge(IStatusReportRepository statusReportRepository, IProjectRepository projectRepository)
        {
            StatusReportRepository = statusReportRepository;
            ProjectRepository = projectRepository;
        }

        public void UpsertStatus(IList<Etl.Csv.StatusCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);
            // things to check
            // all projects exist and project id's returned
            var grpProjects = items.GroupBy(item => item.Project);//.OrderBy(i => i.ToString());

            IList<string> projectNames = new List<string>();
            foreach (var g in grpProjects)
            {
                projectNames.Add(g.Key);
            }
            // below is sql retrieval optimization but makes updating more complicated
            var projects = this.ProjectRepository.GetProjectsByNames(projectNames);

            foreach (var project in projects)
            {
                _logger.Debug("Project found: {0}", project.Name);
                projectNames.Remove(project.Name);
                
            }
            
            // get all status dates to see if we are overwriting an existing report, if so, delete the old one
            var grpStatusDates = items.GroupBy(item => item.StatusDate);

            // items.ToList().ForEach(item => item.
        }
    }
}

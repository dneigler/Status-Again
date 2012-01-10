using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Repository;
using NLog;

namespace Status.ETL.Csv
{
    public class CsvResourceAllocationBridge : ICsvResourceAllocationBridge
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IStatusReportRepository StatusReportRepository { get; set; }

        public IProjectRepository ProjectRepository { get; set; }

        public ITopicRepository TopicRepository { get; set; }

        public IResourceRepository ResourceRepository { get; set; }

        public ITeamRepository TeamRepository { get; set; }

        public IDepartmentRepository DepartmentRepository { get; set; }

        public CsvResourceAllocationBridge(IStatusReportRepository statusReportRepository, IProjectRepository projectRepository, ITopicRepository topicRepository, IResourceRepository resourceRepository, ITeamRepository teamRepository, IDepartmentRepository departmentRepository)
        {
            StatusReportRepository = statusReportRepository;
            ProjectRepository = projectRepository;
            TopicRepository = topicRepository;
            ResourceRepository = resourceRepository;
            TeamRepository = teamRepository;
            DepartmentRepository = departmentRepository;
        }

        public void UpsertResourceAllocations(IList<ResourceAllocationCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);

            // TODO: implement
        }
    }
}

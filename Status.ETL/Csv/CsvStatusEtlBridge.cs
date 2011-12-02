using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Status.ETL.Csv
{
    public class CsvStatusEtlBridge : ICsvStatusEtlBridge
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public void UpsertStatus(IList<Etl.Csv.StatusCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);
            // things to check
            // all projects exist and project id's returned
            var grpProjects = items.GroupBy(item => item.Project).OrderBy(i => i.ToString());

            foreach (var project in grpProjects)
            {
                _logger.Debug("Project found: {0}", project);
            }
            // items.ToList().ForEach(item => item.
        }
    }
}

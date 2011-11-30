using System.Collections.Generic;
using System.Linq;
using NLog;
using Status.Etl;
using Status.Model;

namespace Status.BLL
{
    public class StatusReportManager : IStatusReportManager
    {
        private readonly Logger _logger;

        public IRollStatusProcessor RollStatusProcessor { get; set; }

        public IRollStatusDateProcessor RollStatusDateProcessor { get; set; }

        public IStatusEtl StatusEtl { get; set; }

        public StatusReportManager(Logger logger, IRollStatusProcessor rollStatusProcessor, IRollStatusDateProcessor rollStatusDateProcessor, IStatusEtl statusEtl)
        {
            _logger = logger;
            RollStatusProcessor = rollStatusProcessor;
            RollStatusDateProcessor = rollStatusDateProcessor;
            StatusEtl = statusEtl;
        }

        /// <summary>
        /// Rolls the status report to the default date handled by the StatusRollProcessor
        /// </summary>
        /// <param name="report"></param>
        public StatusReport RollStatusReport(StatusReport report)
        {
            var rolledReport = new StatusReport
                                   {
                                       Caption = report.Caption,
                                       PeriodStart = RollStatusDateProcessor.GetStatusReportDate(report.PeriodStart)
                                   };
            report.Items.ToList().ForEach(
                si =>
                    {
                        StatusItem mappedItem = RollStatusProcessor.MapStatusItem(si, rolledReport.PeriodStart);
                        if (mappedItem != null) rolledReport.Items.Add(mappedItem);
                    });
            return rolledReport;
        }
    }
}
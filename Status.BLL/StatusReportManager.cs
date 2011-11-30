using System.Collections.Generic;
using System.Linq;
using Status.Etl;
using Status.Model;
using NLog;


namespace Status.BLL
{
    public class StatusReportManager : IStatusReportManager
    {
        private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public IRollStatusProcessor RollStatusProcessor { get; set; }

        public IRollStatusDateProcessor RollStatusDateProcessor { get; set; }

        public IStatusEtl StatusEtl { get; set; }

        public StatusReportManager(IRollStatusProcessor rollStatusProcessor, IRollStatusDateProcessor rollStatusDateProcessor, IStatusEtl statusEtl)
        {
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
            _logger.Debug("Rolling status report from {0:yyyy-mm-dd} started", report.PeriodStart);
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
            _logger.Info("Rolled status report from {0:yyyy-mm-dd} to {1:yyyy-mm-dd}", report.PeriodStart, rolledReport.PeriodStart);
            return rolledReport;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Status.Model;

namespace Status.BLL
{
    public class StatusReportManager : IStatusReportManager
    {
        private IRollStatusProcessor _rollStatusProcessor;

        public IRollStatusProcessor RollStatusProcessor
        {
            get
            {
                if (_rollStatusProcessor == null)
                    _rollStatusProcessor = new DefaultRollStatusProcessor();
                return _rollStatusProcessor;
            }
            set { _rollStatusProcessor = value; }
        }

        /// <summary>
        /// Rolls the status report to the default date handled by the StatusRollProcessor
        /// </summary>
        /// <param name="report"></param>
        /// <param name="dateProcessor">The date processor to create new status report date.</param>
        public StatusReport RollStatusReport(StatusReport report, IRollStatusDateProcessor dateProcessor)
        {
            var rolledReport = new StatusReport
                                   {
                                       Caption = report.Caption,
                                       PeriodStart = dateProcessor.GetStatusReportDate(report.PeriodStart),
                                       Items = new List<StatusItem>()
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
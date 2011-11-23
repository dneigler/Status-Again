using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;

namespace Status.BLL
{
    public class StatusReportManager : IStatusReportManager
    {
        private IRollStatusProcessor _rollStatusProcessor = null;

        public IRollStatusProcessor RollStatusProcessor
        {
            get { return _rollStatusProcessor; }
            set { _rollStatusProcessor = value; }
        }

        /// <summary>
        /// Rolls the status report to the default date handled by the StatusRollProcessor
        /// </summary>
        /// <param name="report"></param>
        public StatusReport RollStatusReport(StatusReport report)
        {
            StatusReport rolledReport = new StatusReport();
            rolledReport.Caption = report.Caption;
            rolledReport.PeriodStart = this.RollStatusProcessor.GetPeriodStart(report);
            rolledReport.PeriodEnd = this.RollStatusProcessor.GetPeriodEnd(report);
            rolledReport.Items = new List<StatusItem>();
            report.Items.ToList().ForEach(si => rolledReport.Items.Add(this.RollStatusProcessor.MapStatusItem(si)));
            return rolledReport;
        }

        public void RollStatusReportToDate(StatusReport report, DateTime targetDate) {
        }
    }
}


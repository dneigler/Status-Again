﻿using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using Status.Model;

namespace Status.BLL
{
    public class StatusReportManager : IStatusReportManager
    {
        private IRollStatusProcessor _rollStatusProcessor;

        public IRollStatusProcessor RollStatusProcessor
        {
            get { return _rollStatusProcessor ?? (_rollStatusProcessor = new DefaultRollStatusProcessor()); }
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
                                       PeriodStart = dateProcessor.GetStatusReportDate(report.PeriodStart)
                                   };
            report.Items.ToList().ForEach(
                si =>
                    {
                        StatusItem mappedItem = RollStatusProcessor.MapStatusItem(si, rolledReport.PeriodStart);
                        if (mappedItem != null) rolledReport.Items.Add(mappedItem);
                    });
            return rolledReport;
        }

        public void ImportStatusFromCsv(string fileName)
        {
            var engine = new FileHelperEngine<StatusCsvItem>();
            List<StatusCsvItem> items = engine.ReadFileAsList(fileName);
        }

        public void ExportStatusToCsv(string fileName, IList<StatusReport> reports)
        {
            var engine = new FileHelperEngine<StatusCsvItem>();
            IList<StatusCsvItem> items = new List<StatusCsvItem>();
            // convert reports to items
            engine.WriteFile(fileName, items);
        }

        public void ExportStatusToCsv(string fileName, IList<StatusCsvItem> items)
        {
            var engine = new FileHelperEngine<StatusCsvItem>();
            engine.WriteFile(fileName, items);
        }
    }
}
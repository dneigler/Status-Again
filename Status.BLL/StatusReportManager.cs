using System;
using System.Collections.Generic;
using System.Linq;
using Status.Etl;
using Status.Model;
using NLog;
using Status.Repository;


namespace Status.BLL
{
    public class StatusReportManager : IStatusReportManager
    {
        private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public IRollStatusProcessor RollStatusProcessor { get; set; }

        public IRollStatusDateProcessor RollStatusDateProcessor { get; set; }

        public IStatusEtl StatusEtl { get; set; }

        public IStatusReportRepository StatusReportRepository { get; set; }

        public StatusReportManager(IRollStatusProcessor rollStatusProcessor, IRollStatusDateProcessor rollStatusDateProcessor, IStatusEtl statusEtl, IStatusReportRepository statusReportRepository)
        {
            RollStatusProcessor = rollStatusProcessor;
            RollStatusDateProcessor = rollStatusDateProcessor;
            StatusEtl = statusEtl;
            StatusReportRepository = statusReportRepository;
        }

        /// <summary>
        /// Checks whether there is already a status report in the database for the rolled date.  Rolling 
        /// won't automatically delete an existing report.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="statusRollDate">Output parameter for the date this will be rolled to if valid.</param>
        /// <returns></returns>
        public bool CanRollStatusReport(StatusReport report, out DateTime statusRollDate)
        {
            statusRollDate = this.RollStatusDateProcessor.GetStatusReportDate(report.PeriodStart);
            var sr = this.StatusReportRepository.GetStatusReport(statusRollDate);
            bool canRoll = (sr == null);
            _logger.Debug("CanRollStatusReport called for {0:yyyy-mm-dd} and returned {1}", statusRollDate, canRoll);
            return canRoll;
        }

        /// <summary>
        /// Rolls the status report to the default date handled by the StatusRollProcessor
        /// </summary>
        /// <param name="report"></param>
        /// <param name="auditInfo"> </param>
        public StatusReport RollStatusReport(StatusReport report, AuditInfo auditInfo)
        {
            DateTime statusRollDate;
            if (!CanRollStatusReport(report, out statusRollDate)) throw new Exception("StatusReport already exists, cannot roll to that date");
            _logger.Debug("Rolling status report from {0:yyyy-mm-dd} started", report.PeriodStart);
            var rolledReport = new StatusReport
                                   {
                                       Caption = report.Caption,
                                       PeriodStart = statusRollDate,
                                       AuditInfo = auditInfo
                                   };

            report.Items.ToList().ForEach(
                si =>
                {
                    var mappedItem = RollStatusProcessor.MapStatusItem(si, rolledReport.PeriodStart);
                    // somehow the topic (and project too?) is an issue when mapping this item and storing via 
                    // nhibernate.  the statusreport needs to share the same session.
                    if (mappedItem != null) rolledReport.Items.Add(mappedItem);
                });

            // var sess = this.StatusReportRepository.Session;
            
            using (var txn = this.StatusReportRepository.BeginTransaction())
            {
                try
                {
                    this.StatusReportRepository.Update(rolledReport);
                    txn.Commit();
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("RollStatusReport error", exc);
                    txn.Rollback();
                    throw;
                }
            }
            _logger.Info("Rolled status report from {0:yyyy-mm-dd} to {1:yyyy-mm-dd}", report.PeriodStart, rolledReport.PeriodStart);
            return rolledReport;
        }
    }
}
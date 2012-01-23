using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Etl;
using Status.Model;
using Status.Repository;

namespace Status.BLL
{
    public interface IStatusReportManager
    {
        IRollStatusProcessor RollStatusProcessor { get; set; }
        IRollStatusDateProcessor RollStatusDateProcessor { get; set; }
        IStatusEtl StatusEtl { get; set; }
        IStatusReportRepository StatusReportRepository { get; set; }

        /// <summary>
        /// Rolls the status report to the default date handled by the StatusRollProcessor
        /// </summary>
        /// <param name="report"></param>
        /// <param name="auditInfo"> </param>
        StatusReport RollStatusReport(StatusReport report, AuditInfo auditInfo);

        /// <summary>
        /// Checks whether there is already a status report in the database for the rolled date.  Rolling 
        /// won't automatically delete an existing report.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        bool CanRollStatusReport(StatusReport report, out DateTime statusRollDate);
    }
}

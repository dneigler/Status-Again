using System;
using System.Collections.Generic;
using Status.Model;

namespace Status.Repository
{
    public interface IStatusReportRepository : IRepository
    {
        StatusReport GetActiveStatusReport();

        StatusReport GetStatusReport(DateTime statusDate);

        IList<StatusReport> GetStatusReports(DateTime from, DateTime to);

        void DeleteStatusReport(DateTime statusDate);

        void AddStatusReport(StatusReport statusReport);
    }
}
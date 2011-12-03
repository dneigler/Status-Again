using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class StatusReportRepository : RepositoryBase, IStatusReportRepository
    {
        public StatusReportRepository(string connectionString) : base(connectionString)
        {
        }

        #region IStatusReportRepository Members

        public StatusReport GetActiveStatusReport()
        {
            // need to find the first status report, we'll stick to Mondays for now
            DateTime statusDate = GetCurrentStatusReportDate();
            return GetStatusReport(statusDate);
        }

        public StatusReport GetStatusReport(DateTime statusDate)
        {
            using (var session = CreateSession())
            {
                StatusReport query = (from s in session.Query<StatusReport>()
                                      where s.PeriodStart.Equals(statusDate)
                                      select s).Single();
                return query;
            }
        }

        public IList<StatusReport> GetStatusReports(DateTime @from, DateTime to)
        {
            using (var session = CreateSession())
            {
                var query = (from s in session.Query<StatusReport>()
                                      where s.PeriodStart >= @from && s.PeriodStart <= to
                                      select s).ToList();
                return query;
            }
        }

        public void DeleteStatusReport(DateTime statusDate)
        {
            using (var session = CreateSession())
            {
                StatusReport query = (from s in session.Query<StatusReport>()
                                      where s.PeriodStart.Equals(statusDate)
                                      select s).Single();
                if (query == null)
                    throw new NullReferenceException(String.Format("Status Report for {0} not found", statusDate));
                session.Delete(query);
            }
        }

        #endregion

        public DateTime GetCurrentStatusReportDate()
        {
            DateTime statusDate = DateTime.Today;
            while (statusDate.DayOfWeek != DayOfWeek.Monday)
                statusDate = statusDate.AddDays(-1);
            return statusDate;
        }
    }
}
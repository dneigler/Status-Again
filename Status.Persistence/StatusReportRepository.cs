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
        #region Constructors

        public StatusReportRepository(string connectionString)
            : base(connectionString)
        {
        }

        public StatusReportRepository(ISession session)
            : base(session)
        {
        }

        public StatusReportRepository(ITransaction transaction)
            : base(transaction)
        {
        }

        public StatusReportRepository(string connectionString, ISession session)
            : base(connectionString, session)
        {
        }
        #endregion

        #region IStatusReportRepository Members

        public StatusReport GetActiveStatusReport()
        {
            // need to find the first status report, we'll stick to Mondays for now
            DateTime statusDate = GetCurrentStatusReportDate();
            return GetStatusReport(statusDate);
        }

        public StatusReport GetStatusReport(DateTime statusDate)
        {
            var session = GetSession();
            {
                StatusReport query = (from s in session.Query<StatusReport>()
                                      where s.PeriodStart.Equals(statusDate)
                                      select s).SingleOrDefault();
                return query;
            }
        }

        public IList<StatusReport> GetStatusReports(DateTime @from, DateTime to)
        {
            var session = GetSession();
            {
                var query = (from s in session.Query<StatusReport>()
                             where s.PeriodStart >= @from && s.PeriodStart <= to
                             select s).ToList();
                return query;
            }
        }

        public void DeleteStatusReport(DateTime statusDate)
        {
            var session = GetSession();
            {
                StatusReport query = (from s in session.Query<StatusReport>()
                                      where s.PeriodStart == statusDate
                                      select s).SingleOrDefault();
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
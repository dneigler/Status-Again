using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class ResourceAllocationRepository : RepositoryBase<ResourceAllocation>, IResourceAllocationRepository
    {
        public ResourceAllocationRepository(string connectionString) : base(connectionString)
        {
        }

        public ResourceAllocationRepository(ISession session) : base(session)
        {
        }

        public ResourceAllocationRepository(string connectionString, ISession session) : base(connectionString, session)
        {
        }

        public ResourceAllocationRepository(ITransaction transaction) : base(transaction)
        {
        }

        public IList<ResourceAllocation> GetResourceAllocationsByTeam(int teamId)
        {
            var query = (from ra in this.Session.Query<ResourceAllocation>()
                         where ra.Project.Team.Id.Equals(teamId)
                         select ra);
            return query.ToList();
        }

        public IList<ResourceAllocation> GetResourceAllocationsByTeamDateRange(int teamId, DateTime @from, DateTime? to)
        {
            var query = (from ra in this.Session.Query<ResourceAllocation>()
                         where
                            ra.Project.Team.Id.Equals(teamId) &&
                            (ra.Month >= @from && ra.Month <= (to ?? DateTime.Today))
                         select ra);
            return query.ToList();
        }

        public IList<ResourceAllocation> GetResourceAllocationsByDateRange(DateTime @from, DateTime? to)
        {
            var query = (from ra in this.Session.Query<ResourceAllocation>()
                         where ra.Month >= @from && ra.Month <= (to ?? DateTime.Today)
                         select ra);
            return query.ToList();
        }

        public void DeleteByResourceMonth(Resource resource, DateTime month)
        {
            var query = (from ra in this.Session.Query<ResourceAllocation>()
                         where ra.Month == @month &&
                               ra.Employee == resource
                         select ra);
            query.ForEach(ra => this.Session.Delete(ra));
        }
    }
}
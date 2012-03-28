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
                            ra.Employee.Team.Id == teamId &&
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

        public IList<ProjectAllocation> GetProjectAllocationsByDateRange(DateTime @from, DateTime? to)
        {
            var query = (from ra in this.Session.Query<ResourceAllocation>()
                         where ra.Month >= @from && ra.Month <= (to ?? DateTime.Today)
                         select ra).GroupBy(ra => ra.Project);
            IList<ProjectAllocation> projects = new List<ProjectAllocation>();

            query.ForEach(projGroup =>
            {
                // group by month
                var months = projGroup.GroupBy(pg => pg.Month);
                months.ForEach(monthGroup =>
                {
                    ProjectAllocation pa = new ProjectAllocation();
                    pa.Project = projGroup.Key;
                    pa.Month = monthGroup.Key;
                    pa.Allocation = monthGroup.Sum(ra => ra.Allocation);
                    projects.Add(pa);
                });
            });
            return projects;
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
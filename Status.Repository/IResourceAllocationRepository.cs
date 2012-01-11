using System;
using System.Collections.Generic;
using Status.Model;

namespace Status.Repository
{
    public interface IResourceAllocationRepository : IRepository<ResourceAllocation>
    {
        IList<ResourceAllocation> GetResourceAllocationsByTeam(int teamId);

        IList<ResourceAllocation> GetResourceAllocationsByTeamDateRange(int teamId, DateTime from, DateTime? to);

        IList<ResourceAllocation> GetResourceAllocationsByDateRange(DateTime from, DateTime? to);

        void DeleteByResourceMonth(Resource resource, DateTime month);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Status.Persistence;
using AutoMapper;
using Status.Model;

namespace StatusMvc.Models
{
    public class ResourceAllocationViewModel
    {
        public class TeamAllocationRAVM
        {
            public TeamAllocationRAVM()
            {
                this.Members = new List<UserRAVM>();
            }

            public int Id { get; set; }

            public string Name { get; set; }

            public IList<UserRAVM> Members { get; set; }
            
            public string LeadFullName { get; set; }

            public string LeadId { get; set; }

            /// <summary>
            /// Class shows allocations by user, project, month w/ allocations.
            /// </summary>
            public class UserRAVM
            {
                public UserRAVM()
                {
                    this.Projects = new List<ProjectRAVM>();

                }

                public int Id { get; set; }

                public string FullName { get; set; }

                public IList<ProjectRAVM> Projects { get; set; }
            }

            public class ProjectRAVM
            {
                public ProjectRAVM()
                {
                    this.MonthlyAllocations = new List<MonthRAVM>();

                }

                public int Id { get; set; }

                public string Name { get; set; }

                public IList<MonthRAVM> MonthlyAllocations { get; set; }
            }

            public class ProjectRAVMResolver : ValueResolver<int, IList<ProjectRAVM>>
            {
                private IList<ResourceAllocation> _resourceAllocations;

                public ProjectRAVMResolver(IList<ResourceAllocation> resourceAllocations) {
                    _resourceAllocations = resourceAllocations;
                }

                protected override IList<ProjectRAVM> ResolveCore(int userId)
                {
                    // use repository to load this

                    var allocs = (from ra in _resourceAllocations
                            where ra.Resource.Id == userId
                            select ra);
                    // need to map
                    var projectAllocs = allocs.GroupBy(ra => ra.Project);
                    // project / month allocations
                    
                    // next grouping is by 
                    return null;
                }
            }
            public class MonthRAVM
            {
                public DateTime Month { get; set; }

                public int Id { get; set; }

                public decimal Allocation { get; set; }

            }
        }
    }
}
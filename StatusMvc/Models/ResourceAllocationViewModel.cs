using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StatusMvc.Models
{
    public class ResourceAllocationViewModel
    {
        public class TeamAllocationViewModel
        {
            public TeamAllocationViewModel()
            {
                this.Users = new List<UserAllocationViewModel>();
            }

            public int TeamId { get; set; }

            public string TeamName { get; set; }

            public IList<UserAllocationViewModel> Users { get; set; }

            /// <summary>
            /// Class shows allocations by user, project, month w/ allocations.
            /// </summary>
            public class UserAllocationViewModel
            {
                public UserAllocationViewModel()
                {
                    this.Projects = new List<ProjectViewModel>();

                }

                public int ResourceId { get; set; }

                public string ResourceFullName { get; set; }

                public IList<ProjectViewModel> Projects { get; set; }
            }
            public class ProjectViewModel
            {
                public ProjectViewModel()
                {
                    this.MonthlyAllocations = new List<MonthAllocationViewModel>();

                }

                public int ProjectId { get; set; }

                public string ProjectName { get; set; }

                public IList<MonthAllocationViewModel> MonthlyAllocations { get; set; }
            }

            public class MonthAllocationViewModel
            {
                public DateTime Month { get; set; }

                public int ResourceAllocationId { get; set; }

                public decimal Allocation { get; set; }

            }
        }
    }
}
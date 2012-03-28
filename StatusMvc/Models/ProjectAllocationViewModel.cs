using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Status.Model;

namespace StatusMvc.Models
{
    public class ProjectAllocationViewModel
    {
        public IList<DateTime> Months { get; set; }

        public IList<BudgetTypePAVM> BudgetTypes { get; set; }

        public ProjectAllocationViewModel()
        {
            this.Months = new List<DateTime>();
            this.BudgetTypes = new List<BudgetTypePAVM>();
        }

        public class BudgetTypePAVM : BaseMonthAllocationPAVM
        {
            public ProjectType BudgetType { get; set; }

            public IList<ProjectPAVM> Projects { get; set; }

            public BudgetTypePAVM()
            {
                this.Projects = new List<ProjectPAVM>();
                
            }
        }

        public class ProjectPAVM : BaseMonthAllocationPAVM
        {
            public string ProjectName { get; set; }
        }

        public class BaseMonthAllocationPAVM
        {
            public DateTime Month { get; set; }
            public double Allocation { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class ResourceAllocation : IIdentityColumn
    {
        public virtual int Id { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual DateTime Month { get; set; }

        public virtual Project Project { get; set; }

        public virtual decimal Allocation { get; set; }
    }
}

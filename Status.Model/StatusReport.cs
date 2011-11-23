using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class StatusReport
    {
        public virtual int Id { get; set; }
        public virtual DateTime PeriodStart { get; set; }
        public virtual DateTime PeriodEnd { get; set; }
        public virtual string Caption { get; set; }
        public virtual IList<StatusItem> Items { get; set; }
    }
}

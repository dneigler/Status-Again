using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Employee : Resource
    {
        public virtual string EdsId { get; set; }
        public virtual Team Team { get; set; }
        public virtual Title Title { get; set; }
        public virtual AuditInfo AuditInfo { get; set; }

        public Employee()
        {
            this.AuditInfo = AuditInfo.GetAudit();
        }
    }
}

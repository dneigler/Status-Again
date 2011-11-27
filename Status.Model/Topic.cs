using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Topic
    {
        public virtual int Id { get; set; }
        public virtual string Caption { get; set; }
        public virtual AuditInfo AuditInfo { get; set; }

        public Topic()
        {
            this.AuditInfo = AuditInfo.GetAudit();
        }
    }
}

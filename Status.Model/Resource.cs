using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Resource
    {
        public virtual int Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual AuditInfo AuditInfo { get; set; }

        public Resource()
        {
            this.AuditInfo = AuditInfo.GetAudit();
        }
    }
}

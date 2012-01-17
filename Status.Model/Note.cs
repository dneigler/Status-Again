using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Note : IIdentityColumn
    {
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual AuditInfo AuditInfo { get; set; }
    }
}

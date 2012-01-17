using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Topic : IIdentityColumn
    {
        public virtual int Id { get; set; }
        public virtual string Caption { get; set; }
        public virtual string ExternalId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Company : IIdentityColumn
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

    }
}

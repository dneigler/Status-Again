using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Status.Model
{
    public class Team : IIdentityColumn
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Employee Lead { get; set; }
        public virtual Department Department { get; set; }
        public virtual IList<Employee> Members { get; set; }
    }
}

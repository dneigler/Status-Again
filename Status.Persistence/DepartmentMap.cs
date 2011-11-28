using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            References(x => x.Manager);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Status.Model;

namespace Status.Persistence
{
    public class EmployeeMap : SubclassMap<Employee>
    {
        public EmployeeMap()
        {
            // Id(x => x.Id, "EmployeeID");
            Map(x => x.EdsId);
            Map(x => x.Title).CustomType<Title>();
            References(x => x.Team);
            Component(x => x.AuditInfo);
        }
    }
}

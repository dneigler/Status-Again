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
            Map(x => x.WindowsLogin);
            Map(x => x.Title).CustomType<Title>();
            Map(x => x.Description);
            Map(x => x.DisplayName);
            Map(x => x.DistinguishedName);
            Map(x => x.Guid);
            Map(x => x.MiddleName);
            Map(x => x.Name);
            Map(x => x.Sid);
            Map(x => x.SamAccountName);
            Map(x => x.UserPrincipalName);
            Map(x => x.VoiceTelephoneNumber);
            References(x => x.Team)
                .Cascade.All();
        }
    }
}

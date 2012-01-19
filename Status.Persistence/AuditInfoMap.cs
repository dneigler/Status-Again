using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class AuditInfoMap : ComponentMap<AuditInfo>
    {
        public AuditInfoMap()
        {
            Map(x => x.AuditTime)
                .Not.Nullable();
            
            Map(x => x.MachineName);
            References(x => x.Author)
                .Not.Nullable();
                //.Cascade.SaveUpdate();
        }
    }
}

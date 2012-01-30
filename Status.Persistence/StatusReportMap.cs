using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class StatusReportMap : ClassMap<StatusReport>
    {
        public StatusReportMap()
        {
            Id(x => x.Id)
                .UnsavedValue(0);
            Map(x => x.Caption);
            Map(x => x.PeriodStart);
            Map(x => x.PeriodEnd);
            Component(x => x.AuditInfo);

            HasMany(x => x.Items)
                //.Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}

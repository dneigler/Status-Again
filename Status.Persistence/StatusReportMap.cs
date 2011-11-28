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
            Id(x => x.Id);
            Map(x => x.Caption);
            Map(x => x.PeriodStart);
            Map(x => x.PeriodEnd);
            HasMany(x => x.Items)
                .Cascade
                .AllDeleteOrphan();
        }
    }
}

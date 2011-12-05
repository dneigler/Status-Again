using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class StatusItemMap : ClassMap<StatusItem>
    {
        public StatusItemMap()
        {
            Id(x => x.Id);
            Map(x => x.Caption);
            References(x => x.Topic)
                .Not.Nullable();
            Component(x => x.Milestone);
            HasMany(x => x.Notes)
                .Cascade
                .AllDeleteOrphan();
            
        }
    }
}

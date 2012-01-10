using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Status.Model;

namespace Status.Persistence
{
    public class ResourceAllocationMap : ClassMap<ResourceAllocation>
    {
        public ResourceAllocationMap()
        {
            Id(x => x.Id);
            Map(x => x.Allocation);
            Map(x => x.Month);
            References(x => x.Project);
            References(x => x.Resource);
        }
    }
}

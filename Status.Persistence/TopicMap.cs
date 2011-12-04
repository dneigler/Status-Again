using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Status.Model;

namespace Status.Persistence
{
    public class TopicMap : ClassMap<Topic>
    {
        public TopicMap()
        {
            Id(x => x.Id);
            Map(x => x.Caption);
            Map(x => x.ExternalId);
            DiscriminateSubClassesOnColumn("Type");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class ResourceMap : ClassMap<Resource>
    {
        public ResourceMap()
        {
            Id(x => x.Id);
            Map(x => x.EmailAddress);
            Map(x => x.FirstName)
                .Not.Nullable()
                .Length(50);
            Map(x => x.LastName)
                .Not.Nullable()
                .Length(50);
            Map(x => x.FullName)
                .Not.Nullable()
                .Unique()
                .Length(100);
            DiscriminateSubClassesOnColumn("Type");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Status.Model;
using FluentNHibernate.Mapping;

namespace Status.Persistence
{
    public class TeamMap : ClassMap<Team>
    {
        public TeamMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            References(x => x.Lead);
            References(x => x.Department);
            HasMany(x => x.Members)
                .AsBag()
                .LazyLoad()
                .Inverse()
                ;
        }
    }
}   

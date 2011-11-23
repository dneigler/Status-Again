using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Status.Persistence
{
    public class PkNamePrefixConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column(String.Format("{0}Id", instance.EntityType.Name));
        }
    }
}

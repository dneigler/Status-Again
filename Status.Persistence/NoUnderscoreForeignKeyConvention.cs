using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate;

namespace Status.Persistence
{
    public class NoUnderscoreForeignKeyConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            // property == null for many-to-many, one-to-many, join 
            // property != null for many-to-one
            var refName = property == null ? type.Name : property.Name;
            return string.Format("{0}Id", refName);
        }
    }
}

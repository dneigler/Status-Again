using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using Status.Model;

namespace Status.Persistence
{
    public class StoreConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == "Status.Model";
        }

        public override bool IsComponent(Type type)
        {
            return (type == typeof(AuditInfo));
        }
    }
}

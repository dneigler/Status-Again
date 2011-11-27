using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Status.Model;
using FluentNHibernate.Automapping.Alterations;

namespace Status.Persistence
{
    public class ProjectAutoMap : IAutoMappingOverride<Project>
    {

        public void Override(FluentNHibernate.Automapping.AutoMapping<Project> mapping)
        {
            mapping.Map(x => x.Type).CustomType<ProjectType>();
        }
    }

    public class ProjectMap : ClassMap<Project>
    {
        public ProjectMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Budget);
            Map(x => x.Caption);
            Map(x => x.Description);
            Map(x => x.EndDate);
            Map(x => x.JiraLocation)
                .Length(1000);
            Map(x => x.JiraProject);
            Map(x => x.StartDate);
            Map(x => x.WikiLocation)
                .Length(1000);
            Map(x => x.Year);
            Map(x => x.Type).CustomType<ProjectType>();
            References(x => x.Department);
            References(x => x.Team);
            References(x => x.Lead);
            HasMany(x => x.StatusItems)
                .Inverse()
                .Cascade.DeleteOrphan();
            Component(x => x.AuditInfo);
        }
    }
}

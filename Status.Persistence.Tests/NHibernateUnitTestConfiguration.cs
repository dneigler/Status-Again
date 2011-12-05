using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace Status.Persistence.Tests
{
    public class NHibernateUnitTestConfiguration
    {
        private readonly string ConnString;
        private Configuration _config = null;

        public NHibernateUnitTestConfiguration(string connString)
        {
            this.ConnString = connString;
        }

        public void Configure() {
            _config = Fluently.Configure()
              .Database(MsSqlConfiguration
                .MsSql2008
                .ConnectionString(ConnString))
              .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProjectMap>()
              .Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
              .ExposeConfiguration(CreateSchema)
              .BuildConfiguration();
        }

        public void Cleanup()
        {
            // basically drop the schema
            var schemaExport = new SchemaExport(_config);
            schemaExport.Drop(false, true);
        }

        private static void CreateSchema(Configuration cfg)
        {
            var schemaExport = new SchemaExport(cfg);
            schemaExport.Drop(false, true);
            schemaExport.Create(false, true);
        }

        public ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(MsSqlConfiguration
                .MsSql2008
                .ConnectionString(ConnString))
              .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<ProjectMap>()
                .Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
              .BuildSessionFactory();
        }
    }
}

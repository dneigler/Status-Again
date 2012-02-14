using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Ninject.Modules;
using Status.BLL;
using Status.ETL.Csv;
using Status.Etl;
using Status.Etl.Csv;
using Status.Persistence;
using Status.Repository;

namespace StatusMvc.Modules
{
    public class DefaultStatusAgainWebModule : NinjectModule
    {
            private string _connString = null;

            private ISessionFactory _sessionFactory;

            public string ConnString
            {
                get { return _connString; }
                set { _connString = value; }
            }

            public DefaultStatusAgainWebModule(string connString)
            {
                _connString = connString;
            }

        private ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    _sessionFactory = Fluently.Configure()
                        .Database(MsSqlConfiguration
                                      .MsSql2008
                                      .ConnectionString(ConnString))
                        .Mappings(m => m.FluentMappings
                                           .AddFromAssemblyOf<ProjectMap>()
                                           .Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
                        .BuildSessionFactory();

                return _sessionFactory;
            }
        }

            public override void Load()
            {
                Bind<ISessionFactory>().ToConstant(this.SessionFactory).InRequestScope();

                Bind<IStatusReportManager>().To<StatusReportManager>();
                    //.WithConstructorArgument();
                Bind<IRollStatusProcessor>().To<DefaultRollStatusProcessor>();
                Bind<IRollStatusDateProcessor>().To<DefaultRollStatusDateProcessor>();
                Bind<IStatusEtl>().To<CsvStatusEtl>();
                Bind<ICsvStatusEtlBridge>().To<CsvStatusEtlBridge>();

                Bind<IStatusReportRepository>().To<StatusReportRepository>()
                    .WithConstructorArgument("connectionString", ConnString)
                    ;
                Bind<IProjectRepository>().To<ProjectRepository>()
                    .WithConstructorArgument("connectionString", ConnString);
                Bind<ITopicRepository>().To<TopicRepository>()
                    .WithConstructorArgument("connectionString", ConnString);
                Bind<IResourceRepository>().To<ResourceRepository>()
                    .WithConstructorArgument("connectionString", ConnString);
                Bind<ITeamRepository>().To<TeamRepository>()
                    .WithConstructorArgument("connectionString", ConnString);
                Bind<ITagRepository>().To<TagRepository>()
                    .WithConstructorArgument("connectionString", ConnString);
                Bind<IResourceAllocationRepository>().To<ResourceAllocationRepository>()
                    .WithConstructorArgument("connectionString", ConnString);

            }
        }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Ninject.Modules;
using Status.BLL;
using Status.ETL.Csv;
using Status.Persistence;
using Status.Repository;

namespace Status.Etl.Tests
{
    public class DefaultEtlNinjectModule : NinjectModule
    {
        private string _connString = null;

        public string ConnString
        {
            get { return _connString; }
            set { _connString = value; }
        }

        public DefaultEtlNinjectModule(string connString)
        {
            _connString = connString;
        }

        public override void Load()
        {
            Bind<IStatusReportManager>().To<StatusReportManager>();
            Bind<IRollStatusProcessor>().To<DefaultRollStatusProcessor>();
            Bind<IRollStatusDateProcessor>().To<DefaultRollStatusDateProcessor>();
            Bind<IStatusEtl>().To<Csv.CsvStatusEtl>();
            Bind<ICsvStatusEtlBridge>().To<CsvStatusEtlBridge>();

            Bind<IStatusReportRepository>().To<StatusReportRepository>()
                .WithConstructorArgument("connectionString", ConnString);
            Bind<IProjectRepository>().To<ProjectRepository>()
                .WithConstructorArgument("connectionString", ConnString);
            Bind<ITopicRepository>().To<TopicRepository>()
                .WithConstructorArgument("connectionString", ConnString);
            Bind<IResourceRepository>().To<ResourceRepository>()
                .WithConstructorArgument("connectionString", ConnString);
            Bind<ITeamRepository>().To<TeamRepository>()
                .WithConstructorArgument("connectionString", ConnString);

        }
    }
}

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
        const string ConnString = "server=.\\SQLExpress;" +
    "database=StatusAgain;" +
    "Integrated Security=SSPI;";
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
            
        }
    }
}

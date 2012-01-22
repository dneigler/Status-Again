using Ninject.Modules;
using Status.Etl;
using Status.Etl.Csv;
using Status.Persistence;
using Status.Repository;

namespace Status.BLL.Tests
{
    public class DefaultStatusNinjectModule : NinjectModule
    {
        private string _connString;

        public DefaultStatusNinjectModule(string _connString)
        {
            // TODO: Complete member initialization
            this._connString = _connString;
        }

        public string ConnString { get { return _connString; } }

        public override void Load()
        {
            Bind<IStatusReportManager>().To<StatusReportManager>();
            Bind<IRollStatusProcessor>().To<DefaultRollStatusProcessor>();
            Bind<IRollStatusDateProcessor>().To<DefaultRollStatusDateProcessor>();
            Bind<IStatusEtl>().To<CsvStatusEtl>().Named("Csv");
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

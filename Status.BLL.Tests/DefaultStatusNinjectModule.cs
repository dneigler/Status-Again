using Ninject.Modules;

namespace Status.BLL.Tests
{
    public class DefaultStatusNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IStatusReportManager>().To<StatusReportManager>();
            Bind<IRollStatusProcessor>().To<DefaultRollStatusProcessor>();
            Bind<IRollStatusDateProcessor>().To<DefaultRollStatusDateProcessor>();
            // Bind<IStatusEtl>().To<CsvStatusEtl>().Named("Csv");
            
        }
    }
}

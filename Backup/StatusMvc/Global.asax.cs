using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Mvc;
using StatusMvc.Modules;
using NLog;

namespace StatusMvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        /*
    public static ISessionFactory SessionFactory = CreateSessionFactory();
	
	protected static ISessionFactory CreateSessionFactory()
	{
		return new Configuration()
			.Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml"))
			.BuildSessionFactory();
	}
	
	public static ISession CurrentSession
	{
		get{ return (ISession)HttpContext.Current.Items["current.session"]; }
		set { HttpContext.Current.Items["current.session"] = value; }
	}
	
	protected void Global()
	{
		BeginRequest += delegate
		{
			CurrentSession = SessionFactory.OpenSession();
		};
		EndRequest += delegate
		{
			if(CurrentSession != null)
				CurrentSession.Dispose();
		};
	}
         * */

        protected override IKernel CreateKernel()
        {

            var kernel = new StandardKernel(new DefaultStatusAgainWebModule(ConfigurationManager.ConnectionStrings["StatusAgain"].ConnectionString));
            _logger.Info("Created kernel {0}", kernel.GetType());
            return kernel;
        }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            _logger.Info("Application Started");
        }
    }
}
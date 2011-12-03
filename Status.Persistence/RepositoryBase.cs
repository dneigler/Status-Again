using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace Status.Persistence
{
    public class RepositoryBase
    {
        private volatile ISessionFactory _sessionFactory = null;

        public RepositoryBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        protected ISessionFactory CreateSessionFactory()
        {
            if (_sessionFactory == null)
                _sessionFactory = Fluently.Configure()
                    .Database(MsSqlConfiguration
                                  .MsSql2008
                                  .ConnectionString(ConnectionString))
                    .Mappings(m => m.FluentMappings
                                       .AddFromAssemblyOf<ProjectMap>()
                                       .Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
                    .BuildSessionFactory();

            return _sessionFactory;
        }

        protected ISession CreateSession()
        {
            var sessionFactory = CreateSessionFactory();
            return sessionFactory.OpenSession();
        }
    }
}
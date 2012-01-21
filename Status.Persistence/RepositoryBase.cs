using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public abstract class RepositoryBase<T> : IDisposable, IRepository<T> where T : IIdentityColumn
    {
        private volatile ISessionFactory _sessionFactory = null;

        private volatile ISession _session = null;
        private ITransaction _transaction = null;

        #region Constructors

        public RepositoryBase()
        {
            _session = this.GetSessionFactory().OpenSession();
        }

        public RepositoryBase(string connectionString)
        {
            ConnectionString = connectionString;
            _session = this.GetSessionFactory().OpenSession();
        }

        public RepositoryBase(ISession session)
        {
            _session = session;
        }

        public RepositoryBase(ITransaction transaction)
        {
            Transaction = transaction;
        }

        public RepositoryBase(string connectionString, ISession session)
        {
            ConnectionString = connectionString;
            _session = session;
        }

        #endregion


        #region Transaction and Session Management Methods

        public ITransaction BeginTransaction()
        {
            Transaction = _session.BeginTransaction();
            return Transaction;
        }

        public void CommitTransaction()
        {
            // _transaction will be replaced with a new transaction
            // by NHibernate, but we will close to keep a consistent state.
            Transaction.Commit();

            CloseTransaction();
        }

        public void CommitTransaction(ITransaction transaction)
        {
            Transaction = transaction;
            CommitTransaction();
        }

        public void RollbackTransaction()
        {
            // _session must be closed and disposed after a transaction
            // rollback to keep a consistent state.
            Transaction.Rollback();

            CloseTransaction();
            CloseSession();
        }

        public void RollbackTransaction(ITransaction transaction)
        {
            Transaction = transaction;
            RollbackTransaction();
        }

        public void CloseTransaction()
        {
            Transaction.Dispose();
            Transaction = null;
        }

        public void CloseTransaction(ITransaction transaction)
        {
            Transaction = transaction;
            CloseTransaction();
        }

        public void CloseSession()
        {
            _session.Close();
            _session.Dispose();
            _session = null;
        }

        public void CloseSession(ISession session)
        {
            throw new NotImplementedException();
        }

        #endregion
        
        public string ConnectionString { get; set; }

        public ITransaction Transaction
        {
            get { return _transaction; }
            set
            {
                // some checks if we try to overwrite the transaction here
                if (_transaction == value) return; // if the same then we're good already
                if (_transaction != null && _transaction.IsActive) throw new TransactionException("A transaction is already in progress against this repository.");
                _transaction = value;
            }
        }

        protected ISessionFactory GetSessionFactory()
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

        public ISession Session
        {
            get
            {
                if (_session == null)
                {
                    var sessionFactory = GetSessionFactory();
                    _session = sessionFactory.OpenSession();
                }
                return _session;
            }
            set { _session = value; }
        }

        public IList<T> GetAll()
        {
            var query = (from ra in this.Session.Query<T>()
                         select ra);
            return (IList<T>) query.ToList();
        }

        public T Get(int idValue)
        {
            var query = (from item in this.Session.Query<T>()
                         where item.Id ==idValue
                         select item).FirstOrDefault();
            return query;
        }

        public void Add(T itemToAdd)
        {
            this.Session.Save(itemToAdd);
        }

        public void Update(T itemToUpdate)
        {
            this.Session.SaveOrUpdate(itemToUpdate);
        }

        public void Delete(T itemToDelete)
        {
            this.Session.Delete(itemToDelete);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Transaction != null)
            {
                // Commit transaction by default, unless user explicitly rolls it back.
                // To rollback transaction by default, unless user explicitly commits,
                // comment out the line below.
                CommitTransaction();
            }

            if (_session == null) return;
            _session.Flush(); // commit session transactions
            CloseSession();
        }

        #endregion


    }
}
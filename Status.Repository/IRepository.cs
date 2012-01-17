using System.Collections.Generic;
using NHibernate;
using Status.Model;

namespace Status.Repository
{
    public interface IRepository<T> where T : IIdentityColumn
    {
        ITransaction BeginTransaction();
        void CommitTransaction();
        void CommitTransaction(ITransaction transaction);
        void RollbackTransaction();
        void RollbackTransaction(ITransaction transaction);
        void CloseTransaction();
        void CloseTransaction(ITransaction transaction);
        void CloseSession();
        void CloseSession(ISession session);
        ISession Session { get; set; }
        IList<T> GetAll();
        void Add(T itemToAdd);
        void Update(T itemToUpdate);
        void Delete(T itemToDelete);
    }
}
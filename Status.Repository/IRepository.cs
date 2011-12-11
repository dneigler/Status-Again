using NHibernate;

namespace Status.Repository
{
    public interface IRepository<in T>
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
        void Add(T itemToAdd);
        void Update(T itemToUpdate);
        void Delete(T itemToDelete);
    }
}
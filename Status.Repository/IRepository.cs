using NHibernate;

namespace Status.Repository
{
    public interface IRepository
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
        ISession GetSession();
    }
}
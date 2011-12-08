using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        #region Constructors

        public DepartmentRepository()
        {
        }

        public DepartmentRepository(string connectionString) : base(connectionString)
        {
        }

        public DepartmentRepository(ISession session) : base(session)
        {
        }

        public DepartmentRepository(ITransaction transaction) : base(transaction)
        {
        }

        public DepartmentRepository(string connectionString, ISession session) : base(connectionString, session)
        {
        }

        #endregion


        public IList<Department> GetAllDepartments()
        {
            return (from d in this.Session.Query<Department>()
                    select d).ToList();
        }

        public Department GetByName(string departmentName)
        {
            return (from d in this.Session.Query<Department>()
                    where d.Name.Equals(departmentName)
                    select d).SingleOrDefault();
        }

    }
}
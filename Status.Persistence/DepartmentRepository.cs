using System.Collections.Generic;
using Status.Model;
using Status.Repository;

namespace Status.Persistence
{
    public class DepartmentRepository : RepositoryBase, IDepartmentRepository
    {
        public IList<Department> GetAllDepartments()
        {
            throw new System.NotImplementedException();
        }

        public Department GetByName(string departmentName)
        {
            throw new System.NotImplementedException();
        }

        public void AddDepartment(Department department)
        {
            throw new System.NotImplementedException();
        }
    }
}
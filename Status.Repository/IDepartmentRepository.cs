using System.Collections.Generic;
using Status.Model;

namespace Status.Repository
{
    public interface IDepartmentRepository : IRepository
    {
        IList<Department> GetAllDepartments();

        Department GetByName(string departmentName);

        void AddDepartment(Department department);
    }
}
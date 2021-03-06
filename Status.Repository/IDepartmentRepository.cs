﻿using System.Collections.Generic;
using Status.Model;

namespace Status.Repository
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        IList<Department> GetAllDepartments();

        Department GetByName(string departmentName);

    }
}
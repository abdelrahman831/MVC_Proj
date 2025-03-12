using Demo.BLL.DTOS.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.Departments
{
    public interface IDepartmentService
    {
        IEnumerable<DepartmentToReturnDto> GetAllDepartments();
        DepartmentDetailsToReturnDto? GetDepartmentsById(int id);
        int CreateDepartment(DepartmentToCreateDto department);
        int UpdateDepartment(DepartmentToUpdateDto department);
        bool DeleteDepartment(int id);

    }
}

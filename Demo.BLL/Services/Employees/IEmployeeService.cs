using Demo.BLL.DTOS.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.Employees
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeToReturnDto> GetAllEmployees();
        EmployeeDetailsDto? GetEmployeesById(int id);
        int CreateEmployee(EmployeeToCreateDto employee);
        int UpdateEmployee(EmployeeToUpdateDto employee);
        bool DeleteEmployee(int id);

    }
}

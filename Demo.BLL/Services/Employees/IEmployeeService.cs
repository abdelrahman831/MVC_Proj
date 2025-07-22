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
        Task<IEnumerable<EmployeeToReturnDto>> GetAllEmployeesAsync();
        Task<EmployeeDetailsDto?> GetEmployeesByIdAsync(int id);
        Task<int> CreateEmployeeAsync(EmployeeToCreateDto employee);
        Task<int> UpdateEmployeeAsync(EmployeeToUpdateDto employee);
        Task<bool> DeleteEmployeeAsync(int id);

    }
}

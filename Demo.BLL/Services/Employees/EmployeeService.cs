using AutoMapper;
using Castle.Core.Logging;
using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Employees;
using Demo.DAL.Presistance.Repositories.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Demo.BLL.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {

        public IEmployeeRepository _employeeRepository;
        public IMapper _mapper;
        public ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger,IEmployeeRepository employeeRepository,IMapper mapper) //Ask Clr to Create instance
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region Create
        public int CreateEmployee(EmployeeToCreateDto employeeCreateDto)
        {
            try
            {
                //Log.Information("DTO ricevuto: {@Dto}", employeeCreateDto);
                //var employeet = _mapper.Map<Employee>(employeeCreateDto);
                //Log.Information("Entità mappata: {@Employee}", employeet);

            
            Employee employee = new Employee()
            {
                Name = employeeCreateDto.Name,
                Age = employeeCreateDto.Age,
                Address = employeeCreateDto.Address,
                Salary = employeeCreateDto.Salary,
                PhoneNumber = employeeCreateDto.PhoneNumber,
                IsActive = employeeCreateDto.IsActive,
                Email = employeeCreateDto.Email,
                HiringDate = employeeCreateDto.HiringDate,
                Gender = employeeCreateDto.Gender,
                EmployeeType = employeeCreateDto.EmployeeType,
                CreatedBy = 1,
                LastModifiedBy = 1,
                LastModifiedOn = DateTime.UtcNow,
                DepartmentId = employeeCreateDto.DepartmentId
            };

            //employee.CreatedBy = 1;
            //employee.LastModifiedBy = 1;
            //employee.LastModifiedOn = DateTime.UtcNow;

            return _employeeRepository.AddT(employee);  //Number of rows affected
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        #endregion

        #region Delete
        public bool DeleteEmployee(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee is not null)

                return _employeeRepository.DeleteT(employee) > 0;  //Number of rows affected >0 return true
            return false;

        }
        #endregion

        #region Index
        public IEnumerable<EmployeeToReturnDto> GetAllEmployees()
        {
           return _employeeRepository.GetAllQuarable().Include(E => E.Department)
                .Where(E => !E.IsDeleted)
                .Select(employee => new EmployeeToReturnDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Age = employee.Age,
                Salary = employee.Salary,
                IsActive = employee.IsActive,
                Email = employee.Email,
                Gender = employee.Gender.ToString(),
                EmployeeType = employee.EmployeeType.ToString(),
                Department=employee.Department.Name  //Use Lazy loading

            });


            //var employees = query.ToList();
            //var count = query.Count();
            //var firstEmployee = query.FirstOrDefault();
            //return query;
        } 
        #endregion

        #region Details
        public EmployeeDetailsDto? GetEmployeesById(int id)
        {
            var employee = _employeeRepository.GetById(id);
            if (employee is not null)
            {
                return new EmployeeDetailsDto
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Age = employee.Age,
                    Salary = employee.Salary,
                    IsActive = employee.IsActive,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    Address = employee.Address,
                    HiringDate = employee.HiringDate,
                    Gender = employee.Gender.ToString(),
                    EmployeeType = employee.EmployeeType.ToString(),
                    CreatedBy = employee.CreatedBy,
                    CreatedOn = employee.CreatedOn,
                    LastModifiedBy = employee.LastModifiedBy,
                    
                    DepartmentId=employee.DepartmentId// Lazy
                };

            }
            return null!;
        }
        #endregion

        #region Update
        public int UpdateEmployee(EmployeeToUpdateDto employeeUpdateDto)
        {

            Employee employee = new Employee()
            {
                Id = employeeUpdateDto.Id,
                Name = employeeUpdateDto.Name,
                Age = employeeUpdateDto.Age,
                Address = employeeUpdateDto.Address,
                Salary = employeeUpdateDto.Salary,
                PhoneNumber = employeeUpdateDto.PhoneNumber,
                IsActive = employeeUpdateDto.IsActive,
                Email = employeeUpdateDto.Email,
                HiringDate = employeeUpdateDto.HiringDate,
                Gender = employeeUpdateDto.Gender,
                EmployeeType = employeeUpdateDto.EmployeeType,
                CreatedBy = 1,
                LastModifiedBy = 1,
                LastModifiedOn = DateTime.UtcNow,
                DepartmentId = employeeUpdateDto.DepartmentId 
            };
            return _employeeRepository.UpdateT(employee);
        }
        #endregion
    }
}

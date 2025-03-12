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
using Demo.DAL.Presistance.UnitOfWork;

namespace Demo.BLL.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {

        public IEmployeeRepository _employeeRepository;
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;
        public ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger,IMapper mapper,IUnitOfWork unitOfWork) //Ask Clr to Create instance
        {
    
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #region Create
        public int CreateEmployee(EmployeeToCreateDto employeeCreateDto)
        {
            try
            {
                //Log.Information("DTO ricevuto: {@Dto}", employeeCreateDto);
                //var employeet = _mapper.Map<Employee>(employeeCreateDto);
                //Log.Information("Entità mappata: {@Employee}", employeet);


                var employee = _mapper.Map<Employee>(employeeCreateDto);
            //Employee employee = new Employee()
            ////{
            //    Name = employeeCreateDto.Name,
            //    Age = employeeCreateDto.Age,
            //    Address = employeeCreateDto.Address,
            //    Salary = employeeCreateDto.Salary,
            //    PhoneNumber = employeeCreateDto.PhoneNumber,
            //    IsActive = employeeCreateDto.IsActive,
            //    Email = employeeCreateDto.Email,
            //    HiringDate = employeeCreateDto.HiringDate,
            //    Gender = employeeCreateDto.Gender,
            //    EmployeeType = employeeCreateDto.EmployeeType,
            //    CreatedBy = 1,
            //    LastModifiedBy = 1,
            //    LastModifiedOn = DateTime.UtcNow,
            //    DepartmentId = employeeCreateDto.DepartmentId
            //};

                //employee.CreatedBy = 1;
                //employee.LastModifiedBy = 1;
                //employee.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.EmployeeRepository.AddT(employee);//Number of rows affected
            return _unitOfWork.Complete();

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
            var employee = _unitOfWork.EmployeeRepository.GetById(id);
            if (employee is not null)
                _unitOfWork.EmployeeRepository.DeleteT(employee);
            return _unitOfWork.Complete() >0;

        }
        #endregion

        #region Index
        public IEnumerable<EmployeeToReturnDto> GetAllEmployees()
        {
            return _unitOfWork.EmployeeRepository.GetAllQuarable().Include(E => E.Department)
                 .Where(E => !E.IsDeleted)
                 .Select(employee => _mapper.Map<EmployeeToReturnDto>(employee));

        } 
        #endregion

        #region Details
        public EmployeeDetailsDto? GetEmployeesById(int id)
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(id);  
            if (employee is not null)
            {
                return _mapper.Map<EmployeeDetailsDto>(employee);
      

            }
            return null!;
        }
        #endregion

        #region Update
        public int UpdateEmployee(EmployeeToUpdateDto employeeUpdateDto)
        {
            var employye = _mapper.Map<Employee>(employeeUpdateDto);

             _unitOfWork.EmployeeRepository.UpdateT(employye);
            return _unitOfWork.Complete();
        }
        #endregion
    }
}

using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Entities.Common.Enums;
using Demo.DAL.Entities.Employees;
using Demo.PL.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;
using Serilog;

namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        //Action ==> Master Action
        #region  Service //DependancyInjection
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public EmployeeController(IEmployeeService employeeService,IMapper mapper, ILogger<EmployeeController> logger, IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
            _environment = environment;
        }
        #endregion

        #region Index
        [HttpGet] //As Default
        public IActionResult Index()
        {
            var employees = _employeeService.GetAllEmployees();
            return View(employees);
        } 
        #endregion

        #region Create
        [HttpGet]
        //show the form
        public IActionResult Create( )
        {
            //Send Department from action
        //    ViewData["Departments"] = departmentService.GetAllDepartments();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter
        public IActionResult Create(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)

                return View(employeeVM);
            var message = string.Empty;
            try
            {
                //Log.Information("DTO ricevuto: {@Dto}", employeeVM);

                var result = _mapper.Map<EmployeeToCreateDto>(employeeVM);

                //Log.Information("Entità mappata: {@Employee}", result);
                var Result = _employeeService.CreateEmployee(result);


                //var Result = _employeeService.CreateEmployee(new EmployeeToCreateDto(){
                //    Name = employeeVM.Name,
                //    Age = employeeVM.Age,
                //    Address = employeeVM.Address,
                //    Salary = employeeVM.Salary,
                //    PhoneNumber = employeeVM.PhoneNumber,
                //    IsActive = employeeVM.IsActive,
                //    Email = employeeVM.Email,
                //    HiringDate = employeeVM.HiringDate,
                //    Gender = employeeVM.Gender,
                //    EmployeeType = employeeVM.EmployeeType,
                //    DepartmentId = employeeVM.DepartmentId
                //});


                if (Result > 0)
                {
                    TempData["Message"] = "Congratolations! , Employee is created";

                    return RedirectToAction("Index");
                }
                else
                {
                    message = "Employee is not created";
                    TempData["Message"] = message;
                    ModelState.AddModelError(string.Empty, message);
                    return View(employeeVM);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                if (_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View();
                }
                else
                {
                    message = "An error occurred while creating the Employee";
                    return View("Error", message);


                }

            }
        }
        #endregion

        #region Details
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var employee = _employeeService.GetEmployeesById(id.Value);
            if (employee is null)
            {
                return NotFound();
            }
            return View(employee);

        }
        #endregion

        #region Update
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var employee = _employeeService.GetEmployeesById(id.Value);
            if (employee is null)
            {
                return NotFound();
            }
            
            return View(_mapper.Map<EmployeeViewModel>(employee));
        //    {
             
        //        EmployeeType = Enum.TryParse<EmployeeType>(employee.EmployeeType, out var empType) ? empType : default,
        //        Gender = Enum.TryParse<Gender>(employee.Gender, out var gender) ? gender : default,
        //        Name = employee.Name,
        //        Address = employee.Address,
        //        Email = employee.Email,
        //        Age = employee.Age,
        //        IsActive = employee.IsActive,
        //        PhoneNumber = employee.PhoneNumber,
        //        HiringDate = employee.HiringDate,
        //        Salary = employee.Salary,
        //        DepartmentId=employee.DepartmentId
               
            
        //    });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter

        public IActionResult Edit(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)
            {

                return View(employeeVM);
            }
            var message = string.Empty;
            try
            {
                var Result = _employeeService.UpdateEmployee(_mapper.Map<EmployeeToUpdateDto>(employeeVM));
                //{
                //    Id = id,
                //    Name = employeeVM.Name,
                //    Age = employeeVM.Age,
                //    Address = employeeVM.Address,
                //    Salary = employeeVM.Salary,
                //    PhoneNumber = employeeVM.PhoneNumber,
                //    IsActive = employeeVM.IsActive,
                //    Email = employeeVM.Email,
                //    HiringDate = employeeVM.HiringDate,
                //    Gender = employeeVM.Gender,
                //    EmployeeType = employeeVM.EmployeeType,
                //    DepartmentId = employeeVM.DepartmentId

                //});
                if (Result > 0)
                {
                    TempData["Message"] = "Congratolations! , Employee is Updated";

                    return RedirectToAction("Index");
                }
                else
                {
                    message = "Employee is not updated Ya Man !";
                    TempData["Message"] = message;
                    ModelState.AddModelError(string.Empty, message);
                    return View(employeeVM);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View(employeeVM);
                }
                else
                {
                    message = "An error occurred while updating the Employee";
                    return View("Error", message);

                }

            }

        }
        #endregion

        #region Delete
        [HttpGet]    //Way01 Delete
        public IActionResult Delete(int? id)
        {
            var employee = _employeeService.GetEmployeesById(id.Value);
            if (employee is null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter

        public IActionResult Delete(int id)
        {
            var message = string.Empty;
            try
            {
                var Result = _employeeService.DeleteEmployee(id);

                if (Result == true)
                {
                    TempData["Message"] = "Sure!, Employee is Deleted";

                    return RedirectToAction("Index");
                }
                else
                {
                    message = "Employee is not deleted Ya Man!";
                    TempData["Message"] = message;

                    ModelState.AddModelError(string.Empty, message);
                    return View("Index");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View();
                }
                else
                {
                    message = "An error occurred while deleting the Employee";
                    return View("Error", message);

                }

            }

        } 
        #endregion
    }
}

using AutoMapper;
using Dapper;
using Demo.BLL.DTOS;
using Demo.BLL.DTOS.Employees;
using Demo.BLL.Services.DashBoard;
using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Entities.Employees;
using Demo.PL.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Demo.PL.Controllers
{
    [Authorize]

    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IActivityService _activityService;
        private readonly IDepartmentService _departmentService;

        public EmployeeController(IDepartmentService departmentService,IActivityService activityService, IEmployeeService employeeService, IMapper mapper, IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = Log.ForContext<EmployeeController>();
            _environment = environment;
            _activityService = activityService;
            _departmentService = departmentService;
        }

        private async Task SaveLogToDb(string level, string message, string exception = null)
        {
            using (var connection = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=mvcproj_mvcproj_;User Id=mvcproj_mvcproj_;Password=mvcproj;TrustServerCertificate=True;MultipleActiveResultSets=true"))
            {
                var query = "INSERT INTO Logs (LogLevel, Message, Exception) VALUES (@LogLevel, @Message, @Exception)";
                await connection.ExecuteAsync(query, new { LogLevel = level, Message = message, Exception = exception });
            }
        }


        #region Search Employees
        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string searchValue)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.ToLower();
                employees = employees
                    .Where(e => e.Name.ToLower().Contains(searchValue) || e.Email.ToLower().Contains(searchValue));
            }

            return PartialView("~/Views/Employee/Partials/_EmployeeTablePartial.cshtml", employees);

        }
        #endregion



        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.Information("Fetching all employees");
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }
        #endregion

        #region Create GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewData["Departments"] = new SelectList(departments, "Id", "Name");
            return View(new EmployeeViewModel());
        }
        #endregion

        #region Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)
            {
                await SaveLogToDb("EMPWarning", "Failed to create employee", employeeVM.ToString());
                return View(employeeVM);
            }

            try
            {

                var employeeDto = _mapper.Map<EmployeeToCreateDto>(employeeVM);
                var result = await _employeeService.CreateEmployeeAsync(employeeDto);

                if (result > 0)
                {
                    await _activityService.AddActivity(new DashBoardActivityDto
                    {
                        LogLevel = "EMPCREATEINFO",
                        Message = $"Employee created successfully: {employeeVM.Name}",
                        Status = true,
                        Exception = employeeVM.Name,
                        CreatedAt = DateTime.Now
                    });

                    TempData["Message"] = "Employee created successfully!";
                    return RedirectToAction("Index");
                }

                await SaveLogToDb("EMPWarning", "Failed to create employee", employeeVM.ToString());
                ModelState.AddModelError(string.Empty, "Failed to create employee.");
                return View(employeeVM);
            }
            catch (Exception ex)
            {
                await SaveLogToDb("EMPError", "Error creating employee", ex.Message);
                return View("Error", "An error occurred while creating the employee.");
            }
        }
        #endregion

        #region Details GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching details for employee ID: {Id}", id);
            var employee = await _employeeService.GetEmployeesByIdAsync(id.Value);
            return employee == null ? NotFound() : View(employee);
        }
        #endregion

        #region Edit GET
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            try
            {

                var employee = await _employeeService.GetEmployeesByIdAsync(id.Value);
                var employeevm = _mapper.Map<EmployeeViewModel>(employee);

                return employeevm == null ? NotFound() : View(employeevm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching employee for edit: ID {Id}", id);
                return RedirectToAction("Index");
            }

        }
        #endregion

        #region Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employeeVM)
        {
            var id = employeeVM.Id;
            if (!ModelState.IsValid)
            {
                await SaveLogToDb("EMPWarning", "Failed to update employee", employeeVM.ToString());
                return View(employeeVM);
            }

            try
            {
                if (employeeVM.Id == 0 && employeeVM is null)
                {
                    await SaveLogToDb("EMPEDITWarning", "Failed to update employee", employeeVM.ToString());
                    ModelState.AddModelError(string.Empty, "Failed to update employee.");
                    return View(employeeVM);
                }
                else
                {
                    var result = await _employeeService.UpdateEmployeeAsync(_mapper.Map<EmployeeViewModel, EmployeeToUpdateDto>(employeeVM));
                    if (result > 0)
                    {
                        await _activityService.AddActivity(new DashBoardActivityDto
                        {
                            LogLevel = "EMPEDITINFO",
                            Message = $"Employee updated successfully: {employeeVM.Name}",
                            Status = true,
                            Exception = employeeVM.Name,
                            CreatedAt = DateTime.Now
                        });
                        TempData["Message"] = "Employee updated successfully!";
                        return RedirectToAction("Index");
                    }
                }

                await SaveLogToDb("EMPWarning", "Failed to update employee", employeeVM.ToString());
                ModelState.AddModelError(string.Empty, "Failed to update employee.");
                return View(employeeVM);
            }
            catch (Exception ex)
            {
                await SaveLogToDb("EMPError", "Error updating employee", ex.Message);
                return View("Error", "An error occurred while updating the employee.");
            }
        }
        #endregion

        #region Delete GET
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching employee for deletion: ID {Id}", id);
            var employee = await _employeeService.GetEmployeesByIdAsync(id.Value);
            return employee == null ? NotFound() : View(employee);
        }
        #endregion

        #region Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeesByIdAsync(id);
                var result = await _employeeService.DeleteEmployeeAsync(id);
                if (result)
                {
                    await _activityService.AddActivity(new DashBoardActivityDto
                    {
                        LogLevel = "EMPDELETEINFO",
                        Message = $"Employee deleted successfully: ID {id}",
                        Status = true,
                        Exception = employee.Name,
                        CreatedAt = DateTime.Now
                    });
                    TempData["Message"] = "Employee deleted successfully!";
                    return RedirectToAction("Index");
                }

                await SaveLogToDb("EMPWarning", "Failed to delete employee", id.ToString());
                ModelState.AddModelError(string.Empty, "Failed to delete employee.");
                return View("Index");
            }
            catch (Exception ex)
            {
                await SaveLogToDb("EMPError", "Error deleting employee", ex.Message);
                return View("Error", "An error occurred while deleting the employee.");
            }
        } 
        #endregion
    }
}

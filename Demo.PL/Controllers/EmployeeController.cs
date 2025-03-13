using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.BLL.Services.Employees;
using Demo.PL.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;
        private readonly IWebHostEnvironment _environment;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper, IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = Log.ForContext<EmployeeController>();
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.Information("Fetching all employees");
            var employees = _employeeService.GetAllEmployees();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for EmployeeViewModel: {@EmployeeVM}", employeeVM);
                return View(employeeVM);
            }

            try
            {
                _logger.Information("Creating new employee: {@EmployeeVM}", employeeVM);
                var employeeDto = _mapper.Map<EmployeeToCreateDto>(employeeVM);
                var result = _employeeService.CreateEmployee(employeeDto);

                if (result > 0)
                {
                    TempData["Message"] = "Employee created successfully!";
                    return RedirectToAction("Index");
                }

                _logger.Warning("Failed to create employee: {@EmployeeVM}", employeeVM);
                ModelState.AddModelError(string.Empty, "Failed to create employee.");
                return View(employeeVM);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating employee: {@EmployeeVM}", employeeVM);
                return View("Error", "An error occurred while creating the employee.");
            }
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching details for employee ID: {Id}", id);
            var employee = _employeeService.GetEmployeesById(id.Value);
            return employee == null ? NotFound() : View(employee);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            try
            {
                _logger.Information("Fetching employee for edit: ID {Id}", id);
                var employee = _employeeService.GetEmployeesById(id.Value);
                return employee == null ? NotFound() : View(_mapper.Map<EmployeeViewModel>(employee));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching employee for edit: ID {Id}", id);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for updating EmployeeViewModel: {@EmployeeVM}", employeeVM);
                return View(employeeVM);
            }

            try
            {
                _logger.Information("Updating employee: {@EmployeeVM}", employeeVM);
                var result = _employeeService.UpdateEmployee(_mapper.Map<EmployeeToUpdateDto>(employeeVM));
                if (result > 0)
                {
                    TempData["Message"] = "Employee updated successfully!";
                    return RedirectToAction("Index");
                }

                _logger.Warning("Failed to update employee: {@EmployeeVM}", employeeVM);
                ModelState.AddModelError(string.Empty, "Failed to update employee.");
                return View(employeeVM);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating employee: {@EmployeeVM}", employeeVM);
                return View("Error", "An error occurred while updating the employee.");
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching employee for deletion: ID {Id}", id);
            var employee = _employeeService.GetEmployeesById(id.Value);
            return employee == null ? NotFound() : View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.Information("Deleting employee: ID {Id}", id);
                var result = _employeeService.DeleteEmployee(id);
                if (result)
                {
                    TempData["Message"] = "Employee deleted successfully!";
                    return RedirectToAction("Index");
                }

                _logger.Warning("Failed to delete employee: ID {Id}", id);
                ModelState.AddModelError(string.Empty, "Failed to delete employee.");
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting employee: ID {Id}", id);
                return View("Error", "An error occurred while deleting the employee.");
            }
        }
    }
}

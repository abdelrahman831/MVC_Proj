using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.BLL.Services.Employees;
using Demo.DAL.Entities.Employees;
using Demo.PL.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public EmployeeController(IEmployeeService employeeService, IMapper mapper, IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = Log.ForContext<EmployeeController>();
            _environment = environment;
        }


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



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.Information("Fetching all employees");
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
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
                var result =await _employeeService.CreateEmployeeAsync(employeeDto);

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
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching details for employee ID: {Id}", id);
            var employee =await _employeeService.GetEmployeesByIdAsync(id.Value);
            return employee == null ? NotFound() : View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            try
            {
                _logger.Information("Fetching employee for edit: ID {Id}", id);
                var employee =await _employeeService.GetEmployeesByIdAsync(id.Value);
                var employeevm = _mapper.Map<EmployeeViewModel>(employee);
                _logger.Information("Employee fetched for edit: {@EmployeeVM}", employeevm);
                _logger.Information("Edit GET - Employee ID: {Id}", employeevm.Id);
                return employeevm == null ? NotFound() : View(employeevm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching employee for edit: ID {Id}", id);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employeeVM)
        {
            var id = employeeVM.Id;
            if (!ModelState.IsValid)
            {
                
                return View(employeeVM);
            }

            try
            {
                if (employeeVM.Id == 0 && employeeVM is null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update employee.");
                    return View(employeeVM);
                }
                else
                {
                    var result = await _employeeService.UpdateEmployeeAsync(_mapper.Map<EmployeeViewModel, EmployeeToUpdateDto>(employeeVM));
                    if (result > 0)
                    {
                        TempData["Message"] = "Employee updated successfully!";
                        return RedirectToAction("Index");
                    }
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching employee for deletion: ID {Id}", id);
            var employee = await _employeeService.GetEmployeesByIdAsync(id.Value);
            return employee == null ? NotFound() : View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.Information("Deleting employee: ID {Id}", id);
                var result =await  _employeeService.DeleteEmployeeAsync(id);
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

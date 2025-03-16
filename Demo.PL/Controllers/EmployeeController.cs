using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.BLL.Services.Employees;
using Demo.PL.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            //if (!id.HasValue)
            //    return BadRequest();

            //try
            //{
            //    _logger.Information("Fetching employee for edit: ID {Id}", id);
            //    var employee =await _employeeService.GetEmployeesByIdAsync(id.Value);

            //    return employee == null ? NotFound() : View();
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error(ex, "Error fetching employee for edit: ID {Id}", id);
            //    return RedirectToAction("Index");
            //}

            if (id is null)
            {
                return BadRequest();
            }
            var employee = await _employeeService.GetEmployeesByIdAsync(id.Value);
            if (employee is null)
            {
                return NotFound();
            }
            var employeevm = _mapper.Map<EmployeeDetailsDto, EmployeeViewModel>(employee);
            //   employeevm.Image = employee.Image;
            return View(employeevm);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(EmployeeViewModel employeeVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.Warning("Invalid model state for updating EmployeeViewModel: {@EmployeeVM}", employeeVM);
        //        return View(employeeVM);
        //    }

        //    try
        //    {
        //        _logger.Information("Updating employee: {@EmployeeVM}", employeeVM);
        //        var result =await  _employeeService.UpdateEmployeeAsync(_mapper.Map<EmployeeToUpdateDto>(employeeVM));
        //        if (result > 0)
        //        {
        //            TempData["Message"] = "Employee updated successfully!";
        //            return RedirectToAction("Index");
        //        }

        //        _logger.Warning("Failed to update employee: {@EmployeeVM}", employeeVM);
        //        ModelState.AddModelError(string.Empty, "Failed to update employee.");
        //        return View(employeeVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Error updating employee: {@EmployeeVM}", employeeVM);
        //        return View("Error", "An error occurred while updating the employee.");
        //    }
        //}


        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter

        public async Task<IActionResult> Edit(EmployeeViewModel employeeVM)
        {
            if (!ModelState.IsValid)
            {

                return View(employeeVM);
            }
            var message = string.Empty;
            try
            {
                var Employeeupdated = _mapper.Map<EmployeeViewModel, EmployeeToUpdateDto>(employeeVM);

                var Result = await _employeeService.UpdateEmployeeAsync(Employeeupdated);

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

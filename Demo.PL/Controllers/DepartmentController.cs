using AutoMapper;
using Demo.BLL.DTOS.Departments;
using Demo.BLL.Services.Departments;
using Demo.PL.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;
        private readonly IWebHostEnvironment _environment;

        public DepartmentController(IDepartmentService departmentService, IMapper mapper, IWebHostEnvironment environment)
        {
            _departmentService = departmentService;
            _mapper = mapper;
            _logger = Log.ForContext<DepartmentController>();
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.Information("Fetching all departments");
            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel departmentVM)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for DepartmentViewModel: {@DepartmentVM}", departmentVM);
                return View(departmentVM);
            }

            try
            {
                _logger.Information("Creating new department: {@DepartmentVM}", departmentVM);
                var result = _departmentService.CreateDepartment(_mapper.Map<DepartmentToCreateDto>(departmentVM));

                if (result > 0)
                {
                    TempData["Message"] = "Department created successfully!";
                    return RedirectToAction("Index");
                }

                _logger.Warning("Failed to create department: {@DepartmentVM}", departmentVM);
                ModelState.AddModelError(string.Empty, "Failed to create department.");
                return View(departmentVM);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating department: {@DepartmentVM}", departmentVM);
                return View("Error", "An error occurred while creating the department.");
            }
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching details for department ID: {Id}", id);
            var department = _departmentService.GetDepartmentsById(id.Value);
            return department == null ? NotFound() : View(department);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            try
            {
                _logger.Information("Fetching department for edit: ID {Id}", id);
                var department = _departmentService.GetDepartmentsById(id.Value);
                return department == null ? NotFound() : View(_mapper.Map<DepartmentViewModel>(department));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching department for edit: ID {Id}", id);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DepartmentViewModel departmentVM)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning("Invalid model state for updating DepartmentViewModel: {@DepartmentVM}", departmentVM);
                return View(departmentVM);
            }

            try
            {
                _logger.Information("Updating department: {@DepartmentVM}", departmentVM);
                var result = _departmentService.UpdateDepartment(_mapper.Map<DepartmentToUpdateDto>(departmentVM));

                if (result > 0)
                {
                    TempData["Message"] = "Department updated successfully!";
                    return RedirectToAction("Index");
                }

                _logger.Warning("Failed to update department: {@DepartmentVM}", departmentVM);
                ModelState.AddModelError(string.Empty, "Failed to update department.");
                return View(departmentVM);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating department: {@DepartmentVM}", departmentVM);
                return View("Error", "An error occurred while updating the department.");
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching department for deletion: ID {Id}", id);
            var department = _departmentService.GetDepartmentsById(id.Value);
            return department == null ? NotFound() : View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.Information("Deleting department: ID {Id}", id);
                var result = _departmentService.DeleteDepartment(id);
                if (result)
                {
                    TempData["Message"] = "Department deleted successfully!";
                    return RedirectToAction("Index");
                }

                _logger.Warning("Failed to delete department: ID {Id}", id);
                ModelState.AddModelError(string.Empty, "Failed to delete department.");
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting department: ID {Id}", id);
                return View("Error", "An error occurred while deleting the department.");
            }
        }
    }
}
using AutoMapper;
using Dapper;
using Demo.BLL.DTOS;
using Demo.BLL.DTOS.Departments;
using Demo.BLL.Services.DashBoard;
using Demo.BLL.Services.Departments;
using Demo.PL.ViewModels.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Serilog;

namespace Demo.PL.Controllers
{
    [Authorize]

    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IActivityService _activityService;


        public DepartmentController(IActivityService activityService, IDepartmentService departmentService, IMapper mapper, IWebHostEnvironment environment)
        {
            _departmentService = departmentService;
            _mapper = mapper;
            _logger = Log.ForContext<DepartmentController>();
            _environment = environment;
            _activityService = activityService;
        }

        private async Task SaveLogToDb(string level, string message, string exception = null)
        {
            using (var connection = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=mvcproj_mvcproj_;User Id=mvcproj_mvcproj_;Password=mvcproj;TrustServerCertificate=True;MultipleActiveResultSets=true"))
            {
                var query = "INSERT INTO Logs (LogLevel, Message, Exception) VALUES (@LogLevel, @Message, @Exception)";
                await connection.ExecuteAsync(query, new { LogLevel = level, Message = message, Exception = exception });
            }
        }


        #region Search Department
        [HttpGet]
        public async Task<IActionResult> SearchDepartments(string searchValue)
        {
            var department = await _departmentService.GetAllDepartmentsAsync();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.ToLower();
                department = department
                    .Where(e => e.Name.ToLower().Contains(searchValue) || e.Code.ToLower().Contains(searchValue));
            }

            return PartialView("~/Views/Department/Partials/_DepartmentTablePartial.cshtml", department);

        } 
        #endregion

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _logger.Information("Fetching all departments");
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }
        #endregion

        #region Create GET
        [HttpGet]
        public IActionResult Create() => View();
        #endregion

        #region Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel departmentVM)
        {

            if (!ModelState.IsValid)
            {
                await SaveLogToDb("DEPWarning", "Invalid model state for creating DepartmentViewModel: {@DepartmentVM}", departmentVM.ToString());
                return View(departmentVM);
            }

            try
            {

                var result = await _departmentService.CreateDepartmentAsync(_mapper.Map<DepartmentToCreateDto>(departmentVM));

                if (result > 0)
                {
                    TempData["Message"] = "Department created successfully!";
                    await _activityService.AddActivity(new DashBoardActivityDto
                    {
                        LogLevel = "DEPCREATEINFO",
                        Message = $"Department {departmentVM.Name} created successfully!",
                        CreatedAt = DateTime.Now,
                        Exception = departmentVM.Name
                    });
                    return RedirectToAction("Index");
                }

                await SaveLogToDb("DEPWarning", "Failed to create department: {@DepartmentVM}", departmentVM.ToString());

                ModelState.AddModelError(string.Empty, "Failed to create department.");
                return View(departmentVM);
            }
            catch (Exception ex)
            {
                await SaveLogToDb("DEPError", $"Error creating department: {departmentVM}", ex.Message);
                return View("Error", "An error occurred while creating the department.");
            }
        }
        #endregion

        #region Details GET
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching details for department ID: {Id}", id);
            var department = await _departmentService.GetDepartmentsByIdAsync(id.Value);
            return department == null ? NotFound() : View(department);
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
                _logger.Information("Fetching department for edit: ID {Id}", id);
                var department = await _departmentService.GetDepartmentsByIdAsync(id.Value);
                return department == null ? NotFound() : View(_mapper.Map<DepartmentViewModel>(department));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching department for edit: ID {Id}", id);
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentViewModel departmentVM)
        {
            if (!ModelState.IsValid)
            {
                await SaveLogToDb("DEPWarning", "Invalid model state for updating DepartmentViewModel: {@DepartmentVM}", departmentVM.ToString());
                return View(departmentVM);
            }

            try
            {

                var result = await _departmentService.UpdateDepartmentAsync(_mapper.Map<DepartmentToUpdateDto>(departmentVM));

                if (result > 0)
                {
                    await _activityService.AddActivity(new DashBoardActivityDto
                    {
                        LogLevel = "DEPEDITINFO",
                        Message = $"Department {departmentVM.Name} updated successfully!",
                        CreatedAt = DateTime.Now,
                        Exception = departmentVM.Name
                    });
                    TempData["Message"] = "Department updated successfully!";
                    return RedirectToAction("Index");
                }

                await SaveLogToDb("DEPWarning", "Failed to update department: {@DepartmentVM}", departmentVM.ToString());
                ModelState.AddModelError(string.Empty, "Failed to update department.");
                return View(departmentVM);
            }
            catch (Exception ex)
            {
                await SaveLogToDb("DEPError", $"Error updating department: {departmentVM}", ex.InnerException.Message);
                return View("Error", "An error occurred while updating the department.");
            }
        }
        #endregion

        #region Delete GET
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            _logger.Information("Fetching department for deletion: ID {Id}", id);
            var department = _departmentService.GetDepartmentsByIdAsync(id.Value);
            return department == null ? NotFound() : View(department);
        }
        #endregion

        #region Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                var result = await _departmentService.DeleteDepartmentAsync(id);
                if (result)
                {
                    var department = await _departmentService.GetDepartmentsByIdAsync(id);
                    await _activityService.AddActivity(new DashBoardActivityDto
                    {
                        LogLevel = "DEPDELETEINFO",
                        Message = $"Department ID {id} deleted successfully!",
                        CreatedAt = DateTime.Now,
                        Exception = department.Name
                    });
                    TempData["Message"] = "Department deleted successfully!";
                    return RedirectToAction("Index");
                }

                await SaveLogToDb("DEPWarning", "Failed to delete department: ID {Id}", id.ToString());
                ModelState.AddModelError(string.Empty, "Failed to delete department.");
                return View("Index");
            }
            catch (Exception ex)
            {
                await SaveLogToDb("DEPError", $"Error deleting department: ID {id}", ex.Message);
                return View("Error", "An error occurred while deleting the department.");
            }
        } 
        #endregion

    }
}
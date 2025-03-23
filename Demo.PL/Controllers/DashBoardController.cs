using Dapper;
using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.DashBoard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{

    [Authorize(Roles= "ROOT")]

    public class DashBoardController(UserManager<ApplicationUser> _userManager,IDepartmentService _departmentService,IEmployeeService _employeeService) : Controller
    {

        private async Task<List<ActivityViewModel>> ActivityData()
        {
            using (var connection = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=mvcproj_mvcproj_;User Id=mvcproj_mvcproj_;Password=mvcproj;TrustServerCertificate=True;MultipleActiveResultSets=true"))
            {
                var query = "select * from UserActivity";
                var result=  connection.Query(query).ToList();

                var dataToReturn=  result.Select(r=> new ActivityViewModel 
                { 
                    LogLevel = r.LogLevel,
                    Status = r.Status,
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    Exception = r.Exception
                });

                return dataToReturn.ToList();
            }
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            int totlUsers = users.Count();



            int RegisteredUsersToday = users.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date == DateTime.Now.Date).Count();

            int LoggedInUsers = users.Where(u => u.LastLogin.HasValue && u.LastLogin.Value.Date == DateTime.Now.Date).Count();

            int TotalDepartments = (await _departmentService.GetAllDepartmentsAsync()).Count();

            int TotalEmployees = (await _employeeService.GetAllEmployeesAsync()).Count();

            var dashBoardViewModel = new DashBoardViewModel
            {
                TotalUsers = totlUsers,
                RegisteredUsersToday = RegisteredUsersToday,
                LoggedInUsers = LoggedInUsers,
                TotalDepartments = TotalDepartments,
                TotalEmployees = TotalEmployees
            };



            return View(dashBoardViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AllActivity()
        {
            var allActivity = await ActivityData();
            
            var activity = allActivity.Select(r => new ActivityViewModel
            {
                LogLevel = r.LogLevel,
                Status = r.Status,
                Message = r.Message,
                CreatedAt = r.CreatedAt,
                Exception = r.Exception

            }).ToList();

            return View(activity);
        }



    }
}
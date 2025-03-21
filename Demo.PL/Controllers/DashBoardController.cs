using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.DashBoard;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
    public class DashBoardController(UserManager<ApplicationUser> _userManager,IDepartmentService _departmentService,IEmployeeService _employeeService) : Controller
    {

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
    }
}
using Dapper;
using Demo.BLL.Services.DashBoard;
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

    public class DashBoardController(IDashBoardService _dashBoardService,IActivityService _activityService) : Controller
    {



        public async Task<IActionResult> Index()
        {
            var totlUsers = await _dashBoardService.GetTotalUsersAsync();
            var RegisteredUsersToday = await _dashBoardService.GetAllRegisteredUsersToday();
            var LoggedInUsers = await _dashBoardService.GetLoggedInUsersAsync();
            var TotalDepartments = await _dashBoardService.GetTotalDepartmentsAsync();
            var TotalEmployees = await _dashBoardService.GetTotalEmployeesAsync();
            var allActivity = await _dashBoardService.GetLastTenActivitiesAsync();

            var dashBoardViewModel = new DashBoardViewModel
            {
                TotalUsers = totlUsers,
                RegisteredUsersToday = RegisteredUsersToday,
                LoggedInUsers = LoggedInUsers,
                TotalDepartments = TotalDepartments,
                TotalEmployees = TotalEmployees,
                Activity = allActivity.Select(r => new ActivityViewModel
                {
                    LogLevel = r.LogLevel,
                    Status = r.Status,
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    Exception = r.Exception
                }).ToList()
            };



            return View(dashBoardViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AllActivity()
        {
            var allActivity = await _activityService.GetAllActivitiesAsync();

            var activityViewModel = allActivity.Select(r => new ActivityViewModel
            {
                LogLevel = r.LogLevel,
                Status = r.Status,
                Message = r.Message,
                CreatedAt = r.CreatedAt,
                Exception = r.Exception
            }).ToList();

            return View(activityViewModel);
        }



    }
}
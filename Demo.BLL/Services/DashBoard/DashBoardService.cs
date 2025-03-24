using Demo.BLL.DTOS;
using Demo.DAL.Entities.Identity;
using Demo.DAL.Presistance.Repositories.DashBoardRepositories;
using Demo.DAL.Presistance.Repositories.Departments;
using Demo.DAL.Presistance.Repositories.Employees;
using Demo.DAL.Presistance.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.DashBoard
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashBoardService(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public Task<int> GetAllRegisteredUsersToday()
        {
            var allUsers = _userManager.Users;
            var today = DateTime.Now;
            var registeredToday = allUsers.Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Date == today.Date).Count();
            return Task.FromResult(registeredToday);
        }

        public async Task<IEnumerable<DashBoardActivityDto>> GetLastTenActivitiesAsync()
        {
            var activities = await _unitOfWork.ActivityRepositories.GetAllEnumerableAsync();

            var lastTenActivities = activities.OrderByDescending(x => x.CreatedOn).Take(10).Select(x => new DashBoardActivityDto
            {
                LogLevel = x.LogLevel,
                Status = x.Status,
                Message = x.Message,
                Exception = x.Exception,
                CreatedAt = x.CreatedOn
            });

            return lastTenActivities;
        }

        public Task<int> GetLoggedInUsersAsync()
        {
            var users = _userManager.Users;

            var LoggedInUsers = users.Where(u => u.LastLogin.HasValue && u.LastLogin.Value.Date == DateTime.Now.Date).Count();

            return Task.FromResult(LoggedInUsers);
        }

        public async Task<int> GetTotalDepartmentsAsync()
        {
            var allDepartments =await _unitOfWork.DepartmentRepository.GetAllAsync();
            var totalDepartments = allDepartments.Count();
            return totalDepartments;
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            var allEmployees =await _unitOfWork.EmployeeRepository.GetAllAsync();
            var totalEmployees = allEmployees.Count();
            return totalEmployees;
        }

        public Task<int> GetTotalUsersAsync()
        {
            return Task.FromResult(_userManager.Users.Count());
        }
    }
}

using Demo.BLL.DTOS;
using Demo.DAL.Entities.Identity;
using Demo.DAL.Presistance.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.DashBoard
{
    public class ActivityService : IActivityService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ActivityService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<int> AddActivity(DashBoardActivityDto activity)
        {
            _unitOfWork.ActivityRepositories.AddTAsync(new DAL.Entities.DashBoard.Activity
            {
                LogLevel = activity.LogLevel,
                Status = activity.Status,
                Message = activity.Message,
                Exception = activity.Exception,
                CreatedOn = activity.CreatedAt
            });
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<DashBoardActivityDto>> GetAllActivitiesAsync()
        {
            var activities =await _unitOfWork.ActivityRepositories.GetAllEnumerableAsync();

            var lastTenActivities = activities.OrderByDescending(x => x.CreatedOn).Select(x => new DashBoardActivityDto
            {
                LogLevel = x.LogLevel,
                Status = x.Status,
                Message = x.Message,
                Exception = x.Exception,
                CreatedAt = x.CreatedOn
            });

            return lastTenActivities;

        }


    }

}

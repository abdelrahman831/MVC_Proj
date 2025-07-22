using Demo.BLL.DTOS;
using Demo.DAL.Presistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.DashBoard
{
    public interface IDashBoardService
    {
        Task<int> GetTotalDepartmentsAsync();
        Task<int> GetTotalEmployeesAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetAllRegisteredUsersToday();
        Task<int> GetLoggedInUsersAsync();

        Task<IEnumerable<DashBoardActivityDto>> GetLastTenActivitiesAsync();
    }
}

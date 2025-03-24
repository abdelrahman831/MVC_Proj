using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.BLL.DTOS;

namespace Demo.BLL.Services.DashBoard
{
    public interface IActivityService
    {
        Task<IEnumerable<DashBoardActivityDto>> GetAllActivitiesAsync();

        Task<int> AddActivity(DashBoardActivityDto activity);
    }
}

using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.DAL.Entities.DashBoard;

namespace Demo.DAL.Presistance.Repositories.DashBoardRepositories
{
    public class ActivityRepositories : GenericRepository<Activity>, IActivityRepositories
    {
        public ActivityRepositories(ApplicationDbContext context) : base(context)
        {
        }
    }
}

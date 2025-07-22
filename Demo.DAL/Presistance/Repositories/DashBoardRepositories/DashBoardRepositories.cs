using Demo.DAL.Entities.DashBoard;
using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Presistance.Repositories.DashBoardRepositories
{
    public class DashBoardRepositories : GenericRepository<DashBoard>, IDashBoardRepositorie
    {
        public DashBoardRepositories(ApplicationDbContext context) : base(context)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Entities.DashBoard
{
    public class DashBoard : ModelBase
    {
        public int TotalUsers { get; set; }
        public int RegisteredUsersToday { get; set; }
        public int LoggedInUsers { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalEmployees { get; set; }
    }
}

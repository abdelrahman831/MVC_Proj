using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Entities.DashBoard
{
    public class Activity : ModelBase
    {
        public string LogLevel { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string? Exception { get; set; }
    }
}

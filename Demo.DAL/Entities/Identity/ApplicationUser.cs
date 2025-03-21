using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Entities.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string FName { get; set; }


        public string LName { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastActivity { get; set; }
        public DateTime? CreatedAt { get; set; }

        public bool IsAgree { get; set; }
    }
}

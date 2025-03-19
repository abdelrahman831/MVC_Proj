using Demo.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.EmailService
{
    public interface IEmailService
    {
        public void sendEmail(Email email) { }
    }
}

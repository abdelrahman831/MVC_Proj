using Demo.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.EmailService
{
    public interface IEmailService
    {
        public void SendEmail(Email email) { }
        public void SendHtmlEmail(MailMessage email) { }
    }
}

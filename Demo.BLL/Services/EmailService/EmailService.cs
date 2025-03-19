using Demo.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Demo.BLL.Services.EmailService
{
    public class EmailService : IEmailService
    {



        public void sendEmail(Email email)
        {
            var client = new SmtpClient("smtp@gmail.com",587);

            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("gaberezat.65@gmail.com", "kercxjdafchjparm");
            client.Send("gaberezat.65@gmail.com", email.To, email.Subject, email.Body);
        }


    }
}

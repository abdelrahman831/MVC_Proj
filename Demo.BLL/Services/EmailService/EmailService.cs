using Demo.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace Demo.BLL.Services.EmailService
{
    public class EmailService : IEmailService
    {
        public void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            //sender , reciever
            client.Credentials = new NetworkCredential("mvcprojectemployeerepositorian@gmail.com", "fcmbohlntoscmwol");  //Generate Password 
            client.Send("mvcprojectemployeerepositorian@gmail.com", email.To, email.Subject, email.Body);

        }
    }
}
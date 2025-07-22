using Demo.BLL.Services.EmailService;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Policy;

namespace Demo.PL.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {

        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel contact)
        {
            if(ModelState.IsValid)
            {
                var email = new Email()
                {
                    To = "mvcprojectemployeerepositorian@gmail.com",
                    Subject = contact.Subject,
                    Body = $"New Message received from {contact.Name}\n\n{contact.Message}\n\nSender Email: {contact.Email}",
                    IsBodyHtml = true
                };
                _emailService.SendEmail(email);
                TempData["Message"] = "Your message has been sent successfully";
                return RedirectToAction("Index");
            }


            return RedirectToAction("Index");

        }
    }
}

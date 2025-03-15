using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demo.PL.Controllers
{
    public class AccountController(UserManager<ApplicationUser> _userManager) : Controller
    {

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser()
                {
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email,
                    FName = registerViewModel.FName,
                    LName = registerViewModel.LName,

                    IsAgree = registerViewModel.IsAgree,

                };
                    
                var Result = await _userManager.CreateAsync(newUser,registerViewModel.Password);

                if (Result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(registerViewModel);


        }

        [HttpGet]
        public IActionResult Login(LoginViewModel loginUser)
        {
            return View(loginUser);

        }
    }
}

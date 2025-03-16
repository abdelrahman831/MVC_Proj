using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demo.PL.Controllers
{
    public class AccountController(UserManager<ApplicationUser> _userManager,SignInManager<ApplicationUser> _signinUser) : Controller
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
        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);

                if (user is not null)
                {
                    var check = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                    if (check)
                    {
                        var sign = await _signinUser.PasswordSignInAsync(user,loginViewModel.Password,loginViewModel.RememberMe,false);
                        if (sign.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,"IncorrectPassword");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Name NOt Found");

                }

            }

                return View(loginViewModel);
            
        }

        public async Task<IActionResult> LogOut(LoginViewModel loginViewModel)
        {
            await _signinUser.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}

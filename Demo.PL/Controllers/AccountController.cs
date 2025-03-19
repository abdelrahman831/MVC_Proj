using Demo.BLL.Services.EmailService;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinUser;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailSettings)
        {
            _userManager = userManager;
            _signinUser = signInManager;
            _emailService = emailSettings;
        }



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

                var Result = await _userManager.CreateAsync(newUser, registerViewModel.Password);

                if (Result.Succeeded)
                {
                    TempData["Message"] = "User created successfully!";

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
                        var sign = await _signinUser.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
                        if (sign.Succeeded)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "IncorrectPassword");
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

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordViewModel forgetpwdVm)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(forgetpwdVm.Email);
                if (user is not null)

                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);
                    var email = new Demo.DAL.Entities.Identity.Email()
                    {
                        To = forgetpwdVm.Email,
                        Subject = "Reset Your Password",
                        Body = url
                    };
                    _emailService.SendEmail(email);
                    TempData["Message"] = "The email was successfully sent";


                    TempData["Email"] = user.Email;
                    TempData["Token"] = token;

                    return RedirectToAction("CheckYourInbox");
                    //Send Email 
                }
                ModelState.AddModelError(string.Empty, "Invalid operation");

            }

            return View(forgetpwdVm);
        }

        public IActionResult CheckYourInbox()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["Email"] = email;
            TempData["Token"] = token;

          
                return View();
   
        }

        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["Email"] as string;
                string token = TempData["Token"] as string;
                if (email is not null && token is not null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user is not null)
                    {
                        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.Password);

                        if (result.Succeeded)
                        {
                            TempData["Message"] = "Password reset successfully";
                            return RedirectToAction("Login");
                        }
                        else
                        {

                            ModelState.AddModelError(string.Empty, "An error occured, please try again");

                        }
                    }

                    ModelState.AddModelError(string.Empty, "An error occured, please try again");

                }
                return RedirectToAction("Register");

            }
            return View(resetPasswordViewModel);

        }
    }
}


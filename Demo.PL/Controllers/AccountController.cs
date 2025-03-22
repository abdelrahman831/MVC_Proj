using Demo.BLL.Services.EmailService;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinUser;
        private readonly IEmailService _emailService;
        private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly Serilog.ILogger _logger;


        #region Ctor Ingection
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailSettings)
        {
            _userManager = userManager;
            _signinUser = signInManager;
            _emailService = emailSettings;
        }
        #endregion



        #region Register Get
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        #endregion


       

        #region Login Get
        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }
        #endregion

        


        #region LogOut
        [HttpGet]
        public async Task<IActionResult> LogOut(LoginViewModel loginViewModel)
        {

            ClaimsPrincipal currentUser = this.User;
            var user = await _userManager.GetUserAsync(currentUser);



            if (user is not null)
            {
                user.LastLogin = null;
                await _userManager.UpdateAsync(user);
            }

            await _signinUser.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region ForgetPassword Get
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        #endregion

       

        #region Check Your Inbox Get
        [HttpGet]
        public IActionResult CheckYourInbox()
        {
            return View();
        }
        #endregion

        #region Reset Password Get
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["Email"] = email;
            TempData["Token"] = token;


            return View();

        }
        #endregion

        #region Reset Password Post
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            _logger.Information("Starting password reset process");
            if (ModelState.IsValid)
            {
                string email = TempData["Email"] as string;
                string token = TempData["Token"] as string;
                _logger.Information($"Retrieved TempData - Email: {email}, Token: {token}");

                if (email is not null && token is not null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user is not null)
                    {
                        _logger.Information($"User found for email {email}, attempting password reset");
                        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.Password);

                        if (result.Succeeded)
                        {
                            _logger.Information("Password reset successfully");
                            TempData["Message"] = "Password reset successfully";
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            _logger.Error("Password reset failed due to an error in UserManager");
                            ModelState.AddModelError(string.Empty, "An error occurred, please try again");
                        }
                    }
                    else
                    {
                        _logger.Error("User not found for password reset");
                    }
                }
                _logger.Warning("Email or token missing in TempData, redirecting to Register");
                return RedirectToAction("Register");
            }
            _logger.Warning("Model state is invalid, returning ResetPassword view");
            return View(resetPasswordViewModel);
        }
        #endregion

        #region Send Reset Password Post
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordViewModel forgetpwdVm)
        {
            _logger.Information("Starting password reset email process");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgetpwdVm.Email);
                if (user is not null)
                {
                    _logger.Information($"User found for email {forgetpwdVm.Email}, generating reset token");
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);
                    _logger.Information($"Generated password reset URL: {url}");

                    var email = new Demo.DAL.Entities.Identity.Email()
                    {
                        To = forgetpwdVm.Email,
                        Subject = "Reset Your Password",
                        Body = url
                    };
                    _emailService.SendEmail(email);
                    _logger.Information("Password reset email sent successfully");

                    TempData["Message"] = "The email was successfully sent";
                    TempData["Email"] = user.Email;
                    TempData["Token"] = token;

                    return RedirectToAction("CheckYourInbox");
                }
                _logger.Warning($"User not found for email {forgetpwdVm.Email}");
                ModelState.AddModelError(string.Empty, "Invalid operation");
            }
            _logger.Warning("Model state is invalid, returning ForgetPassword view");
            return View(forgetpwdVm);
        }
        #endregion

        #region Login Post
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            _logger.Information("Starting login process");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);
                if (user is not null)
                {
                    _logger.Information($"User found for email {loginViewModel.UserName}, verifying password");
                    var check = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                    if (check)
                    {
                        _logger.Information("Password verification succeeded, signing in user");
                        var sign = await _signinUser.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
                        if (sign.Succeeded)
                        {
                            user.LastLogin = DateTime.Now;
                            await _userManager.UpdateAsync(user);
                            _logger.Information($"User {user.Email} logged in successfully");
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        _logger.Warning("Password verification failed");
                        ModelState.AddModelError(string.Empty, "IncorrectPassword");
                        TempData["Message"] = "IncorrectPassword";
                    }
                }
                else
                {
                    _logger.Warning($"User not found for email {loginViewModel.UserName}");
                    ModelState.AddModelError(string.Empty, "User Name Not Found");
                    TempData["Message"] = "UserNameNotFound";
                }
            }
            _logger.Warning("Model state is invalid, returning Login view");
            return View(loginViewModel);
        }
        #endregion

        #region Register Post
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            _logger.Information("Starting user registration process");
            if (ModelState.IsValid)
            {
                _logger.Information($"Creating new user: {registerViewModel.Email}");
                var newUser = new ApplicationUser()
                {
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email,
                    FName = registerViewModel.FName,
                    LName = registerViewModel.LName,
                    CreatedAt = DateTime.Now,
                    IsAgree = registerViewModel.IsAgree,
                };

                var result = await _userManager.CreateAsync(newUser, registerViewModel.Password);
                if (result.Succeeded)
                {
                    _logger.Information($"User {registerViewModel.Email} created successfully, assigning role");
                    await _userManager.AddToRoleAsync(newUser, "User");
                    TempData["Message"] = "User created successfully!";
                    return RedirectToAction("Login");
                }
                else
                {
                    _logger.Error($"User creation failed for {registerViewModel.Email}, Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    TempData["Error"] = result.Errors.Select(e => e.Description).ToList();
                }
            }
            _logger.Warning("Model state is invalid, returning Register view");
            return View(registerViewModel);
        }
        #endregion

    }
}


using Demo.BLL.Services.EmailService;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using Dapper;
namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinUser;
        private readonly IEmailService _emailService;
        private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly ILogger _logger;


        #region Ctor Ingection
        public AccountController(ILogger<AccountController> logger,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailSettings)
        {
            _userManager = userManager;
            _signinUser = signInManager;
            _emailService = emailSettings;
            _logger = logger;
        }
        #endregion


        private async Task SaveLogToDb(string level, string message, string exception = null)
        {
            using (var connection = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=mvcproj_mvcproj_;User Id=mvcproj_mvcproj_;Password=mvcproj;TrustServerCertificate=True;MultipleActiveResultSets=true"))
            {
                var query = "INSERT INTO Logs (LogLevel, Message, Exception) VALUES (@LogLevel, @Message, @Exception)";
                await connection.ExecuteAsync(query, new { LogLevel = level, Message = message, Exception = exception });
            }
        }


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
            _logger.LogInformation("Starting password reset process");
            if (ModelState.IsValid)
            {
                string email = TempData["Email"] as string;
                string token = TempData["Token"] as string;
                _logger.LogInformation($"Retrieved TempData - Email: {email}, Token: {token}");

                if (email is not null && token is not null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user is not null)
                    {
                        _logger.LogInformation($"User found for email {email}, attempting password reset");
                        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.Password);

                        if (result.Succeeded)
                        {
                            _logger.LogInformation("Password reset successfully");
                            TempData["Message"] = "Password reset successfully";
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            _logger.LogError("Password reset failed due to an error in UserManager");
                            ModelState.AddModelError(string.Empty, "An error occurred, please try again");
                        }
                    }
                    else
                    {
                        _logger.LogError("User not found for password reset");
                    }
                }
                _logger.LogWarning("Email or token missing in TempData, redirecting to Register");
                return RedirectToAction("Register");
            }
            _logger.LogWarning("Model state is invalid, returning ResetPassword view");
            return View(resetPasswordViewModel);
        }
        #endregion

        #region Send Reset Password Post
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordViewModel forgetpwdVm)
        {
            _logger.LogInformation("Starting password reset email process");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgetpwdVm.Email);
                if (user is not null)
                {
                    _logger.LogInformation($"User found for email {forgetpwdVm.Email}, generating reset token");
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);
                    _logger.LogInformation($"Generated password reset URL: {url}");

                    var email = new Demo.DAL.Entities.Identity.Email()
                    {
                        To = forgetpwdVm.Email,
                        Subject = "Reset Your Password",
                        Body = url
                    };
                    _emailService.SendEmail(email);
                    _logger.LogInformation("Password reset email sent successfully");

                    TempData["Message"] = "The email was successfully sent";
                    TempData["Email"] = user.Email;
                    TempData["Token"] = token;

                    return RedirectToAction("CheckYourInbox");
                }
                _logger.LogWarning($"User not found for email {forgetpwdVm.Email}");
                ModelState.AddModelError(string.Empty, "Invalid operation");
            }
            _logger.LogWarning("Model state is invalid, returning ForgetPassword view");
            return View(forgetpwdVm);
        }
        #endregion

        #region Login Post
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            _logger.LogInformation("[LOGIN] Starting login process for user: {UserName}", loginViewModel.UserName);
            SaveLogToDb("LOGIN", "Starting login process", loginViewModel.UserName);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);
                if (user is not null)
                {
                    _logger.LogInformation("[LOGIN] User found: {Email}. Verifying password...", user.Email);
                    SaveLogToDb("LOGIN", "User found, verifying password", user.Email);

                    var check = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                    if (check)
                    {
                        _logger.LogInformation("[LOGIN] Password verified successfully for user: {Email}. Attempting sign-in...", user.Email);
                        SaveLogToDb("LOGIN", "Password verified, signing in", user.Email);

                        var sign = await _signinUser.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
                        if (sign.Succeeded)
                        {
                            user.LastLogin = DateTime.Now;
                            await _userManager.UpdateAsync(user);
                            _logger.LogInformation("[LOGIN] User {Email} signed in successfully.", user.Email);
                            SaveLogToDb("LOGIN", "User signed in successfully", user.Email);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            _logger.LogWarning("[LOGIN] Sign-in failed for user: {Email}", user.Email);
                            SaveLogToDb("LOGIN", "Sign-in failed", user.Email);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[LOGIN] Password verification failed for user: {Email}", user.Email);
                        SaveLogToDb("LOGIN", "Password verification failed", user.Email);
                        ModelState.AddModelError(string.Empty, "IncorrectPassword");
                        TempData["Message"] = "IncorrectPassword";
                    }
                }
                else
                {
                    _logger.LogWarning("[LOGIN] No user found with email: {Email}", loginViewModel.UserName);
                    SaveLogToDb("LOGIN", "No user found", loginViewModel.UserName);
                    ModelState.AddModelError(string.Empty, "User Name Not Found");
                    TempData["Message"] = "UserNameNotFound";
                }
            }
            else
            {
                _logger.LogWarning("[LOGIN] Model state is invalid, returning Login view");
                SaveLogToDb("LOGIN", "Model state invalid");
            }
            return View(loginViewModel);
        }
        #endregion

        #region Register Post
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            _logger.LogInformation("[REGISTER] Starting registration process for user: {Email}", registerViewModel.Email);
            SaveLogToDb("REGISTER", "Starting registration process", registerViewModel.Email);

            if (ModelState.IsValid)
            {
                _logger.LogInformation("[REGISTER] Creating new user: {Email}", registerViewModel.Email);
                SaveLogToDb("REGISTER", "Creating new user", registerViewModel.Email);

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
                    _logger.LogInformation("[REGISTER] User {Email} created successfully. Assigning role...", registerViewModel.Email);
                    SaveLogToDb("REGISTER", "User created successfully, assigning role", registerViewModel.Email);

                    await _userManager.AddToRoleAsync(newUser, "User");
                    TempData["Message"] = "User created successfully!";
                    return RedirectToAction("Login");
                }
                else
                {
                    _logger.LogError("[REGISTER] User creation failed for {Email}. Errors: {Errors}", registerViewModel.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    SaveLogToDb("REGISTER", "User creation failed", registerViewModel.Email);
                    TempData["Error"] = result.Errors.Select(e => e.Description).ToList();
                }
            }
            else
            {
                _logger.LogWarning("[REGISTER] Model state is invalid, returning Register view");
                SaveLogToDb("REGISTER", "Model state invalid");
            }
            return View(registerViewModel);
        }
        #endregion

    }
}


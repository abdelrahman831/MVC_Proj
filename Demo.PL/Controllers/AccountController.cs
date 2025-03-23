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
using System.Linq.Expressions;
using NuGet.Common;
namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinUser;
        private readonly IEmailService _emailService;
        private readonly RoleManager<ApplicationUser> _roleManager;



        #region Ctor Ingection
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailSettings)
        {
            _userManager = userManager;
            _signinUser = signInManager;
            _emailService = emailSettings;
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

        private async Task SaveLogToDbForDashBoard(string level,byte status, string message, string exception = null)
        {
            using (var connection = new SqlConnection("Server=sql.bsite.net\\MSSQL2016;Database=mvcproj_mvcproj_;User Id=mvcproj_mvcproj_;Password=mvcproj;TrustServerCertificate=True;MultipleActiveResultSets=true"))
            {
                var query = "INSERT INTO UserActivity (LogLevel,Status, Message, Exception) VALUES (@LogLevel,@Status, @Message, @Exception)";
                await connection.ExecuteAsync(query, new { LogLevel = level, Status= status, Message = message, Exception = exception });
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
            try
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
                                await SaveLogToDbForDashBoard("Reset Password", 1, "Password Reset Successfuly", email);

                                TempData["Message"] = "Password reset successfully";
                                return RedirectToAction("Login");
                            }
                            else
                            {
                                await SaveLogToDbForDashBoard("Reset Password", 0, "An error occured while resetting the password", $"{email} -- {result.Errors}");

                                ModelState.AddModelError(string.Empty, "An error occurred, please try again");
                            }
                        }
                        else
                        {
                            await SaveLogToDbForDashBoard("Reset Password", 0, "Email not found in DataBase", email);

                        }
                    }
                    if (email is not null)
                        await SaveLogToDbForDashBoard("Reset Password", 0, "Missing Token", email);
                    if (token is not null)
                        await SaveLogToDbForDashBoard("Reset Password", 0, "Missing Email", token);


                    return RedirectToAction("Register"); 
                }

            }
            catch (Exception ex)
            {
                await SaveLogToDbForDashBoard("Reset Password", 0, "An error occured", ex.Message);

            }
            return View(resetPasswordViewModel);
        }
        #endregion

        #region Send Reset Password Post
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
                }
                ModelState.AddModelError(string.Empty, "Invalid operation");
            }
            return View(forgetpwdVm);
        }
        #endregion

        #region Login Post
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            SaveLogToDb("LOGIN", "Starting login process", loginViewModel.UserName);
            try
            {


                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);
                    if (user is not null)
                    {
                        SaveLogToDb("LOGIN", "User found, verifying password", user.Email);

                        var check = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                        if (check)
                        {
                            SaveLogToDb("LOGIN", "Password verified, signing in", user.Email);

                            var sign = await _signinUser.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
                            if (sign.Succeeded)
                            {
                                user.LastLogin = DateTime.Now;
                                await _userManager.UpdateAsync(user);
                                SaveLogToDb("LOGIN", "User signed in successfully", user.Email);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                SaveLogToDb("LOGIN", "Sign-in failed", user.Email);
                            }
                        }
                        else
                        {
                            SaveLogToDb("LOGIN", "Password verification failed", user.Email);
                            ModelState.AddModelError(string.Empty, "IncorrectPassword");
                            TempData["Message"] = "IncorrectPassword";
                        }
                    }
                    else
                    {
                        SaveLogToDb("LOGIN", "No user found", loginViewModel.UserName);
                        ModelState.AddModelError(string.Empty, "User Name Not Found");
                        TempData["Message"] = "UserNameNotFound";
                    }
                }
                else
                {
                    SaveLogToDb("LOGIN", "Model state invalid");
                }
            }
            catch (Exception ex)
            {
                SaveLogToDb("LOGIN", "An error occurred while logging in", ex.Message);
            }
            return View(loginViewModel);
        }
        #endregion

        #region Register Post
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            SaveLogToDb("REGISTER", "Starting registration process", registerViewModel.Email);

            if (ModelState.IsValid)
            {
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
                    SaveLogToDb("REGISTER", "User created successfully, assigning role", registerViewModel.Email);

                    await _userManager.AddToRoleAsync(newUser, "User");
                    TempData["Message"] = "User created successfully!";
                    return RedirectToAction("Login");
                }
                else
                {
                    SaveLogToDb("REGISTER", "User creation failed", registerViewModel.Email);
                    TempData["Error"] = result.Errors.Select(e => e.Description).ToList();
                }
            }
            else
            {
                SaveLogToDb("REGISTER", "Model state invalid");
            }
            return View(registerViewModel);
        }
        #endregion

    }
}


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
using Demo.BLL.Services.DashBoard;
using Demo.BLL.DTOS;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using Google.Apis.Auth;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinUser;
        private readonly IEmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IActivityService _activityService;


        #region Ctor Ingection
        public AccountController(RoleManager<IdentityRole> roleManager,IActivityService activityService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailSettings)
        {
            _userManager = userManager;
            _signinUser = signInManager;
            _emailService = emailSettings;
            _activityService = activityService;
            _roleManager = roleManager;
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

        [HttpPost]
        public async Task<IActionResult> GoogleLogin(string credential)
        {
            try
            {
                // Validazione del token restituito da Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential);

                // Qui puoi usare i dati dell'utente, ad esempio:
                var userEmail = payload.Email;
                var userName = payload.Name;
                var userId = payload.Subject; // ID univoco Google

                // Se vuoi registrare l'utente nel database, fallo qui

                return Ok(new { Success = true, Email = userEmail, Name = userName });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Errore nella validazione", Error = ex.Message });
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



            if (user is not null && user.Email is not null)
            {
                user.LastLogin = null;
                await _userManager.UpdateAsync(user);

                var activity = new DashBoardActivityDto
                {
                    LogLevel = "LogOut",
                    Status = true,
                    Message = "User Logged Out",
                    Exception = user.Email,
                    CreatedAt = DateTime.Now
                };

                await _activityService.AddActivity(activity);
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
                                var activity = new DashBoardActivityDto
                                {
                                    LogLevel = "ResetPwd",
                                    Status = true,
                                    Message = "Password Reset Successfuly",
                                    Exception = user.Email,
                                    CreatedAt = DateTime.Now
                                };

                                await _activityService.AddActivity(activity);

                                TempData["Message"] = "Password reset successfully";

                                //Send email to confirm password reset
                                var emailTosend = new Demo.DAL.Entities.Identity.Email()
                                {
                                    To = user.Email,
                                    Subject = "Password Reset Confirmation",
                                    Body = $"Hi {user.FName} {user.LName} Your password has been reset successfully"
                                };

                                _emailService.SendEmail(emailTosend);

                                return RedirectToAction("Login");
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    await SaveLogToDb("ResetPwd", "An error occured", error.Description);
                                }

                                ModelState.AddModelError(string.Empty, "An error occurred, please try again");
                            }
                        }
                    }

                    return RedirectToAction("Register"); 
                }

            }
            catch (Exception ex)
            {
                await SaveLogToDb("Reset Password","An error occured", ex.Message);

            }
            return View(resetPasswordViewModel);
        }
        #endregion

        #region Send Reset Password Post
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordViewModel forgetpwdVm)
        {
            try
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

                    var activity = new DashBoardActivityDto
                    {
                        LogLevel = "SenResetPwdUrl",
                        Status = true,
                        Message = "Reset Pwd Link was sent Successfully",
                        Exception = user?.Email,
                        CreatedAt = DateTime.Now
                    };

                    await _activityService.AddActivity(activity);

                    return RedirectToAction("CheckYourInbox");
                }
                await SaveLogToDb("SendResetPwdUrl", "No user found", forgetpwdVm.Email);
                ModelState.AddModelError(string.Empty, "Invalid operation");
            }
            }
            catch (Exception ex)
            {
                await SaveLogToDb("SendResetPwdUrl", "An error occured", ex.Message);
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

                        var check = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                        if (check)
                        {

                            var sign = await _signinUser.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
                            if (sign.Succeeded)
                            {
                                user.LastLogin = DateTime.Now;
                                await _userManager.UpdateAsync(user);

                                var activity = new DashBoardActivityDto
                                {
                                    LogLevel = "LOGIN",
                                    Status = true,
                                    Message = "User Logged In",
                                    Exception = user.Email,
                                    CreatedAt = DateTime.Now
                                };

                                await _activityService.AddActivity(activity);
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
            try
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
                        var activity = new DashBoardActivityDto
                        {
                            LogLevel = "REGISTER",
                            Status = true,
                            Message = "User created successfully",
                            Exception = newUser.Email,
                            CreatedAt = DateTime.Now
                        };

                        await _activityService.AddActivity(activity);

                        SaveLogToDb("REGISTER", "User created successfully, assigning role", registerViewModel.Email);

                        if(!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }
                        await _userManager.AddToRoleAsync(newUser, "User");
                        TempData["Message"] = "User created successfully!";

                        //Send Registration email
                        var email = new Demo.DAL.Entities.Identity.Email()
                        {
                            To = registerViewModel.Email,
                            Subject = "Registration Confirmation",
                            Body = $"Hi {registerViewModel.FName} {registerViewModel.LName} You have successfully registered to our web app"
                        };
                        _emailService.SendEmail(email);

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
            }
            catch (Exception ex)
            {
                SaveLogToDb("REGISTER", "An error occurred while registering", ex.Message);
            }
            return View(registerViewModel);
        }
        #endregion

    }
}


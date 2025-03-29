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
using System.Net.Mail;

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
        public AccountController(RoleManager<IdentityRole> roleManager, IActivityService activityService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailSettings)
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

        //[HttpGet("signin-google")]
        //public async Task<IActionResult> GoogleLogin(string credential)
        //{
        //    try
        //    {
        //        // Validazione del token restituito da Google
        //        var payload = await GoogleJsonWebSignature.ValidateAsync(credential);

        //        // Qui puoi usare i dati dell'utente, ad esempio:
        //        var userEmail = payload.Email;
        //        var userName = payload.Name;
        //        var userId = payload.Subject; // ID univoco Google

        //        // Se vuoi registrare l'utente nel database, fallo qui

        //        return Ok(new { Success = true, Email = userEmail, Name = userName });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Success = false, Message = "Errore nella validazione", Error = ex.Message });
        //    }
        //}


        #region Register Get
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";

                await SaveLogToDb("Register", "An error occurred", ex.Message);
                return View();
            }
        }
        #endregion

        #region Login Get
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";

                await SaveLogToDb("Login", "An error occurred", ex.Message);
                return View();

            }

        }
        #endregion

        #region LogOut
        [HttpGet]
        public async Task<IActionResult> LogOut(LoginViewModel loginViewModel)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";
                await SaveLogToDb("LogOut", "An error occurred", ex.Message);
                return RedirectToAction("Login", "Account");
            }
        }
        #endregion

        #region ForgetPassword Get
        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";
                await SaveLogToDb("ForgetPassword", "An error occurred", ex.Message);
                return View();
            }
        }
        #endregion

        #region Check Your Inbox Get
        [HttpGet]
        public async Task<IActionResult> CheckYourInbox()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";
                await SaveLogToDb("CheckYourInbox", "An error occurred", ex.Message);
                return View();
            }
        }
        #endregion

        #region Reset Password Get
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            try
            {
                if (email is null || token is null)
                {
                    TempData["Error"] = "An error occurred";

                    return RedirectToAction("Register");
                }
                return View(new ResetPasswordViewModel());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";
                await SaveLogToDb("ResetPassword", "An error occurred", ex.Message);
                return RedirectToAction("Register");
            }
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
                    string email = TempData["Email"] as string ?? "NoEmail";
                    string token = TempData["Token"] as string ?? "NoToken";

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
                                var mailMessage = new MailMessage
                                {
                                    Subject = "🔐 Password Reset Confirmation",
                                    IsBodyHtml = true, // Imposta il contenuto come HTML
                                    Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #d1d1d1; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #007bff; text-align: center;'>🔐 Password Reset Successful</h2>
            <p style='color: #333; text-align: center; font-size: 16px;'>
                Hi <strong>{user.FName} {user.LName}</strong>, <br>
                Your password has been reset successfully. You can now log in using your new password.
            </p>
            <div style='text-align: center; margin-top: 20px;'>
                <a href='https://mvcproj.bsite.net/Account/Login' style='text-decoration: none; background-color: #007bff; color: white; padding: 10px 20px; border-radius: 5px; display: inline-block; font-size: 16px;'>
                    🔑 Log In Now
                </a>
            </div>
            <hr style='border: none; border-top: 1px solid #ccc; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                If you didn't request this change, please contact support immediately.
            </p>
        </div>"
                                };

                                mailMessage.To.Add(user.Email);
                                _emailService.SendHtmlEmail(mailMessage);

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
                TempData["Error"] = "An error occurred";
                await SaveLogToDb("Reset Password", "An error occured", ex.Message);
                //Notify the Developer
                var email = new MailMessage()
                {
                    Subject = "🚨 System Error Alert",
                    Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ff4d4d; border-radius: 10px; background-color: #fff5f5;'>
            <h2 style='color: #d9534f; text-align: center;'>⚠️ An Error Occurred!</h2>
            <p style='color: #333; text-align: center;'>
                An error was detected in your system. Below is the error message:
            </p>
            <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; font-size: 14px; color: #721c24;'>
                <strong>Error Message:</strong>
                <pre style='white-space: pre-wrap; word-wrap: break-word; font-family: Consolas, monospace; background-color: #ffe6e6; padding: 10px; border-radius: 5px;'>{ex.Message}</pre>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                Please check the system logs for further details.
            </p>
            <hr style='border: none; border-top: 1px solid #ff4d4d; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                This is an automated alert. Do not reply.
            </p>
        </div>",
                    IsBodyHtml = true

                };
                email.To.Add("abdulelrakh@gmail.com");
                _emailService.SendHtmlEmail(email);

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

                        var mailMessage = new MailMessage
                        {
                            Subject = "Reset Your Password",
                            Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #333; text-align: center;'>Password Reset Request</h2>
            <p style='color: #555; text-align: center;'>
                We received a request to reset your password. Click the button below to set a new password.
            </p>
            <div style='text-align: center; margin: 20px 0;'>
                <a href='{url}' style='background-color: #007bff; color: #ffffff; padding: 12px 20px; text-decoration: none; font-size: 16px; border-radius: 5px; display: inline-block;'>
                    Reset Password
                </a>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                If you did not request this, you can safely ignore this email.
            </p>
            <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                This is an automated email, please do not reply.
            </p>
        </div>",
                            IsBodyHtml = true 
                        };

                        mailMessage.To.Add(forgetpwdVm.Email);

                        _emailService.SendHtmlEmail(mailMessage);

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
                    TempData["Error"] = "No user found with the provided email";
                    return RedirectToAction("Register");
                }
            }
            catch (Exception ex)
            {
                await SaveLogToDb("SendResetPwdUrl", "An error occured", ex.Message);
                TempData["Error"] = "An error occurred";

                var email = new MailMessage()
                {
                    Subject = "🚨 System Error Alert",
                    Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ff4d4d; border-radius: 10px; background-color: #fff5f5;'>
            <h2 style='color: #d9534f; text-align: center;'>⚠️ An Error Occurred!</h2>
            <p style='color: #333; text-align: center;'>
                An error was detected in your system. Below is the error message:
            </p>
            <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; font-size: 14px; color: #721c24;'>
                <strong>Error Message:</strong>
                <pre style='white-space: pre-wrap; word-wrap: break-word; font-family: Consolas, monospace; background-color: #ffe6e6; padding: 10px; border-radius: 5px;'>{ex.Message}</pre>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                Please check the system logs for further details.
            </p>
            <hr style='border: none; border-top: 1px solid #ff4d4d; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                This is an automated alert. Do not reply.
            </p>
        </div>",
                    IsBodyHtml = true

                };
                email.To.Add("abdulelrakh@gmail.com");
                _emailService.SendHtmlEmail(email);
            }

            return View(forgetpwdVm);
        }
        #endregion

        #region Login Post
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            await SaveLogToDb("LOGIN", "Starting login process", loginViewModel.UserName);
            try
            {


                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(loginViewModel.UserName);
                    if (user is not null)
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            await SaveLogToDb("LOGIN", "Email not confirmed", user.Email);
                            ModelState.AddModelError(string.Empty, "Email not confirmed");
                            TempData["Message"] = "EmailNotConfirmed";

                        

                            var otpCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                            var mailMessage = new MailMessage
                            {
                                Subject = "📧 Registration Confirmation - Verify Your Email",
                                IsBodyHtml = true, // Attivare il supporto per HTML nel corpo dell'email
                                Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #d1d1d1; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #007bff; text-align: center;'>📧 Verify Your Email</h2>
            <p style='color: #333; text-align: center; font-size: 16px;'>
                Hi <strong>{user.FName} {user.LName}</strong>, <br>
                Thank you for registering! To complete your registration, please verify your email by entering the following OTP code in the confirmation form:
            </p>
            <div style='text-align: center; font-size: 20px; font-weight: bold; padding: 10px; border-radius: 5px; background-color: #f0f0f0; display: inline-block;'>
                {otpCode}
            </div>
            <div style='text-align: center; margin-top: 20px;'>
                <a href='https://mvcproj.bsite.net/Account/ConfirmRegister' style='text-decoration: none; background-color: #007bff; color: white; padding: 10px 20px; border-radius: 5px; display: inline-block; font-size: 16px;'>
                    🔑 Confirm Email
                </a>
            </div>
            <hr style='border: none; border-top: 1px solid #ccc; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                If you did not request this, please ignore this email.
            </p>
        </div>"
                            };
                            mailMessage.To.Add(user.Email);
                            
                            _emailService.SendHtmlEmail(mailMessage);

                            TempData["Email"] = user.Email;

                            return RedirectToAction("ConfirmRegister");
                        }

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
                                await SaveLogToDb("LOGIN", "Sign-in failed", user.Email);
                            }
                        }
                        else
                        {
                            await SaveLogToDb("LOGIN", "Password verification failed", user.Email);
                            ModelState.AddModelError(string.Empty, "IncorrectPassword");
                            TempData["Message"] = "IncorrectPassword";
                        }
                    }
                    else
                    {
                        await SaveLogToDb("LOGIN", "No user found", loginViewModel.UserName);
                        ModelState.AddModelError(string.Empty, "User Name Not Found");
                        TempData["Message"] = "UserNameNotFound";
                    }
                }
                else
                {
                    await SaveLogToDb("LOGIN", "Model state invalid");
                }
            }
            catch (Exception ex)
            {
                await SaveLogToDb("LOGIN", "An error occurred while logging in", ex.Message);
                TempData["Error"] = "An error occurred";

                var email = new MailMessage()
                {
                    Subject = "🚨 System Error Alert",
                    Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ff4d4d; border-radius: 10px; background-color: #fff5f5;'>
            <h2 style='color: #d9534f; text-align: center;'>⚠️ An Error Occurred!</h2>
            <p style='color: #333; text-align: center;'>
                An error was detected in your system. Below is the error message:
            </p>
            <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; font-size: 14px; color: #721c24;'>
                <strong>Error Message:</strong>
                <pre style='white-space: pre-wrap; word-wrap: break-word; font-family: Consolas, monospace; background-color: #ffe6e6; padding: 10px; border-radius: 5px;'>{ex.Message}</pre>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                Please check the system logs for further details.
            </p>
            <hr style='border: none; border-top: 1px solid #ff4d4d; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                This is an automated alert. Do not reply.
            </p>
        </div>",
                    IsBodyHtml = true

                };
                email.To.Add("abdulelrakh@gmail.com");
                _emailService.SendHtmlEmail(email);
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
                await SaveLogToDb("REGISTER", "Starting registration process", registerViewModel.Email);

                if (ModelState.IsValid)
                {
                    await SaveLogToDb("REGISTER", "Creating new user", registerViewModel.Email);

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

                        await SaveLogToDb("REGISTER", "User created successfully, assigning role", registerViewModel.Email);

                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }
                        await _userManager.AddToRoleAsync(newUser, "User");
                        TempData["Message"] = "User created successfully!";

                        //Send Registration email
                        var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        var email = new MailMessage
                        {
                            Subject = "✨ Registration Confirmation",
                            Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #333; text-align: center;'>🎉 Welcome to Our Web App!</h2>
            <p style='color: #555; text-align: center;'>
                Hi <strong>{registerViewModel.FName} {registerViewModel.LName}</strong>,<br>
                You have successfully registered on our platform. To verify your email, please use the code below:
            </p>
            <div style='text-align: center; margin: 20px 0;'>
                <span style='font-size: 22px; font-weight: bold; color: #007bff; background: #e7f3ff; padding: 10px 20px; border-radius: 5px; display: inline-block;'>
                    {confirmationCode}
                </span>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                Enter this code in the OTP form to confirm your email.
            </p>
            <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                If you did not sign up for this account, please ignore this email.
            </p>
        </div>",
                            IsBodyHtml = true
                        };

                        email.To.Add(registerViewModel.Email);
                        _emailService.SendHtmlEmail(email);



                        TempData["Email"] = registerViewModel.Email;

                        return RedirectToAction("ConfirmRegister");
                    }
                    else
                    {
                        await SaveLogToDb("REGISTER", "User creation failed", registerViewModel.Email);
                        TempData["Error"] = result.Errors.Select(e => e.Description).ToList();
                    }
                }
                else
                {
                    await SaveLogToDb("REGISTER", "Model state invalid");
                }
            }
            catch (Exception ex)
            {
                await SaveLogToDb("REGISTER", "An error occurred while registering", ex.Message);
                TempData["Error"] = "An error occurred";
                var email = new MailMessage()
                {
                    Subject = "🚨 System Error Alert",
                    Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ff4d4d; border-radius: 10px; background-color: #fff5f5;'>
            <h2 style='color: #d9534f; text-align: center;'>⚠️ An Error Occurred!</h2>
            <p style='color: #333; text-align: center;'>
                An error was detected in your system. Below is the error message:
            </p>
            <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; font-size: 14px; color: #721c24;'>
                <strong>Error Message:</strong>
                <pre style='white-space: pre-wrap; word-wrap: break-word; font-family: Consolas, monospace; background-color: #ffe6e6; padding: 10px; border-radius: 5px;'>{ex.Message}</pre>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                Please check the system logs for further details.
            </p>
            <hr style='border: none; border-top: 1px solid #ff4d4d; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                This is an automated alert. Do not reply.
            </p>
        </div>",
                    IsBodyHtml = true

                };
                email.To.Add("abdulelrakh@gmail.com");
                _emailService.SendHtmlEmail(email);
            }
            return View(registerViewModel);
        }
        #endregion

        #region Confirm Register GET
        [HttpGet]
        public async Task<IActionResult> ConfirmRegister()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(TempData["Email"] as string ?? "UNKNOWN");
                if(user is not null)
                {
                    var userConfirmed = _userManager.IsEmailConfirmedAsync(user);
                    if (userConfirmed.Result)
                    {
                        TempData["Message"] = "Email already confirmed";
                        return RedirectToAction("Login");
                    }
                    return View(new ConfirmEmailViewModel());

                }
                TempData["Error"] = "User is not found please register";

                return RedirectToAction("Register");

            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred";
                await SaveLogToDb("ConfirmRegister", "An error occurred", ex.Message);
                return View();
            }
        }
        #endregion

        #region Confirm Register POST
        [HttpPost]
        public async Task<IActionResult> ConfirmRegister(string otp)
        {
            try
            {
                var email = TempData["Email"] as string ?? "UNKNOWN";
                var user = await _userManager.FindByEmailAsync(email);

                if (user is not null)
                {
                    var result = await _userManager.ConfirmEmailAsync(user, otp);
                    if (result.Succeeded)
                    {
                        TempData["Message"] = "Email confirmed successfully";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Error"] = result.Errors.Select(e => e.Description).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                await SaveLogToDb("ConfirmRegister", "An error occurred while confirming email", ex.Message);
                TempData["Error"] = "An error occurred";
                var email = new MailMessage()
                {
                    Subject = "🚨 System Error Alert",
                    Body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ff4d4d; border-radius: 10px; background-color: #fff5f5;'>
            <h2 style='color: #d9534f; text-align: center;'>⚠️ An Error Occurred!</h2>
            <p style='color: #333; text-align: center;'>
                An error was detected in your system. Below is the error message:
            </p>
            <div style='background-color: #f8d7da; padding: 15px; border-radius: 5px; font-size: 14px; color: #721c24;'>
                <strong>Error Message:</strong>
                <pre style='white-space: pre-wrap; word-wrap: break-word; font-family: Consolas, monospace; background-color: #ffe6e6; padding: 10px; border-radius: 5px;'>{ex.Message}</pre>
            </div>
            <p style='color: #888; text-align: center; font-size: 14px;'>
                Please check the system logs for further details.
            </p>
            <hr style='border: none; border-top: 1px solid #ff4d4d; margin: 20px 0;'>
            <p style='color: #888; text-align: center; font-size: 12px;'>
                This is an automated alert. Do not reply.
            </p>
        </div>",
                    IsBodyHtml = true

                };
                email.To.Add("abdulelrakh@gmail.com");
                _emailService.SendHtmlEmail(email);
            }
            return View();
        }
        #endregion
    }

}


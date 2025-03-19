using AutoMapper;
using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Departments;
using Demo.DAL.Presistance.Repositories.Employees;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Demo.PL.Mapping.Profiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Demo.DAL.Presistance.UnitOfWork;
using Demo.PL.Mapping.Profiles.Departments;
using Demo.BLL.Mapping.Profiles.Employees;
using Demo.BLL.Mapping.Profiles.Departments;
using Demo.BLL.Services.Attacments;
using Demo.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Demo.BLL.Services.EmailService;
namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>((options) =>
            {

                options.UseLazyLoadingProxies();
                //  options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);  //old Way
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));  //New Way

            }, ServiceLifetime.Scoped);
            //builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddAutoMapper(M => M.AddProfile(new ViemodelMappingProfiles()));
            builder.Services.AddAutoMapper(M => M.AddProfile(new DepartmentVieModelMappingProfiles()));
            builder.Services.AddAutoMapper(typeof(EmployeeProfile));
            builder.Services.AddAutoMapper(typeof(DepartmentServiceMapping));
            builder.Services.AddTransient<IAttacchmentService, AttachmentService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>((options) =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 5;
            }
                ).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.ExpireTimeSpan = TimeSpan.FromDays(2);
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
                config.AccessDeniedPath = "/Home/Error";
            });

            builder.Services.AddAuthorization();



            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
            //builder.Host.UseSerilog();
            //builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Register}");

            app.Run();
            #region 1 - MVC Project Architecture

            #endregion

            #region 2 - DAL - Department Entity- Department Configurations - DbContext

            #endregion

            #region 3 - DbContext - Dependency Injection

            #endregion

            #region 4 - DAL - Department Repository

            #endregion

            #region 5 - BLL - Department Service - DTOS

            #endregion
            //----------Session 04 -------------------
            #region 1 - Department Controller - Index

            #endregion
            #region 2 - Department Controller - Create

            #endregion

            #region 3 - Department Controller - Details

            #endregion

            #region 4 - Department Controller - Edit

            #endregion

            #region 5 - Department Controller - Delete

            #endregion
            //----------Session 05 -------------------
            #region 1 - Employee Entity - Configs - Migration

            #endregion

            #region 2 - Employee Repository

            #endregion

            #region 3 - Employee Service

            #endregion

            #region 4 - Employee Controller - Index - Create

            #endregion

            #region 5 - Employee Controller - Details

            #endregion

            #region 6 - Employee Controller - Edit

            #endregion

            #region 7 - Employee Controller - Delete

            #endregion
            //----------Session 06 -------------------
            #region 1 - IEnumerable Vs IQueryable

            #endregion

            #region 2 - Client-Side Validation

            #endregion

            #region 3 - AntiForgeryToken [Action Filter]

            #endregion

            #region 4 - Partial Views

            #endregion

            #region 5 - ViewData Vs ViewBag

            #endregion

            #region 6 - TempData

            #endregion

            #region 7 - RelationShip Between Department & Employee

            #endregion

            #region 8 - RelationShip Between Department & Employee Part 2

            #endregion
        }
    }
}
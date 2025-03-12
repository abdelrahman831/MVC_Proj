using AutoMapper;
using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Departments;
using Demo.DAL.Presistance.Repositories.Employees;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Serilog;
using Demo.PL.Mapping.Profiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            });
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddAutoMapper(M => M.AddProfile(new ViemodelMappingProfiles()));


            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

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

using Demo.BLL.Services.Departments;
using Demo.BLL.Services.Employees;
using Demo.DAL.Presistance.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Demo.PL.Mapping.Profiles;
using Demo.DAL.Presistance.UnitOfWork;
using Demo.PL.Mapping.Profiles.Departments;
using Demo.BLL.Mapping.Profiles.Employees;
using Demo.BLL.Mapping.Profiles.Departments;
using Demo.BLL.Services.Attacments;
using Demo.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Demo.BLL.Services.EmailService;
using Demo.BLL.Services.DashBoard;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Authentication.Google;

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

            builder.Services.AddScoped<UserActivityFilter>();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<UserActivityFilter>();
            }); 

            builder.Services.AddAutoMapper(M => M.AddProfile(new ViemodelMappingProfiles()));
            builder.Services.AddAutoMapper(M => M.AddProfile(new DepartmentVieModelMappingProfiles()));
            builder.Services.AddAutoMapper(typeof(EmployeeProfile));
            builder.Services.AddAutoMapper(typeof(DepartmentServiceMapping));
            builder.Services.AddTransient<IAttacchmentService, AttachmentService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IDashBoardService, DashBoardService>();
            builder.Services.AddScoped<IActivityService, ActivityService>();


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


            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day).CreateLogger();




       //     builder.Services.AddAuthentication()
       //.AddGoogle(options =>
       //{
       //    options.ClientId = "779795901287-o7khjtcj0d7014b99mf4v8e3327rfbge.apps.googleusercontent.com";
       //    options.ClientSecret = "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";
       //    options.Events.OnRemoteFailure = context =>
       //    {
       //        Console.WriteLine($"Errore Google OAuth: {context.Failure?.Message} {context.Request}");
       //        context.HandleResponse();
       //        return Task.CompletedTask;
       //    };// Deve corrispondere a quello nella Console Google
       //});

       //     builder.Services.ConfigureApplicationCookie(options =>
       //     {
       //         options.Cookie.SameSite = SameSiteMode.None;
       //         options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
       //     });


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
            app.Use(async (context, next) =>
            {
                context.Response.Cookies.Delete(".AspNetCore.Correlation.Google");
                context.Response.Cookies.Delete(".AspNetCore.OpenIdConnect.Nonce");
                await next();
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.Run();
            
        }
    }
}
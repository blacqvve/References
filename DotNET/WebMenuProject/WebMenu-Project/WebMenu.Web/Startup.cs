using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WebMenu.Data.Data;
using WebMenu.Data.Managers;
using WebMenu.Data.Models;
using WebMenu.Data.Settings;

namespace WebMenu.Web
{
     public class Startup
     {
          private IConfigurationRoot _appSettings;
          private IConfiguration _globalSettings;
          public Startup(IWebHostEnvironment env)
          {
               _appSettings = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();
               _globalSettings = JsonConfiguration.CreateConfigurationContainer();

          }
          public IConfiguration Configuration { get; }

          // This method gets called by the runtime. Use this method to add services to the container.
          public void ConfigureServices(IServiceCollection services)
          {
               APIContext.ConnectionString = _globalSettings.GetConnectionString("Development");
               services.AddControllers();
               services.AddIdentity<User, IdentityRole>()
                   .AddErrorDescriber<CustomIdentityErrorDescriber>()
                   .AddEntityFrameworkStores<APIContext>();
               services.AddDbContext<APIContext>();
               services.AddSingleton(_appSettings);
               services.AddScoped<ICompanyManager, CompanyManager>();
               services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
               services.AddDbContext<APIContext>();
               services.AddControllersWithViews();

               services.Configure<IdentityOptions>(options =>
               {

                 // Default Password settings.
                 options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 0;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;
               });
               services.ConfigureApplicationCookie(options =>
               {
                    options.ExpireTimeSpan = TimeSpan.FromDays(1080);
                    options.Events.OnRedirectToLogin = context =>
                    {
                         context.Response.StatusCode = 401;

                         return Task.CompletedTask;
                    };
               });
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
          {
               if (env.IsDevelopment())
               {
                    app.UseDeveloperExceptionPage();
               }
               else
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

               app.UseEndpoints(endpoints =>
               {
                    endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Home}/{action=Index}/{id?}");
               });
          }
     }
}

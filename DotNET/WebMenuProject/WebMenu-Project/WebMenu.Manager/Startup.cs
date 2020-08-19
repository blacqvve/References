using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMenu.Data.Settings;
using WebMenu.Data.Models;
using WebMenu.Data.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebMenu.Manager.Helpers;
using WebMenu.Data.Managers;

namespace WebMenu.Manager
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
               services.AddIdentity<User, IdentityRole>()
                    .AddDefaultTokenProviders()
                   .AddErrorDescriber<CustomIdentityErrorDescriber>()
                   .AddEntityFrameworkStores<APIContext>();
               services.AddDbContext<APIContext>();
               services.AddHttpContextAccessor();
               services.AddSingleton(_appSettings);
               services.TryAddScoped<ICompanyManager, CompanyManager>();
               services.TryAddScoped<IQrGenerateManager, QrGenerateManager>();
               services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
               services.Configure<CookiePolicyOptions>(options =>
               {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
               });

               services.ConfigureApplicationCookie(options =>
               {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                    options.LoginPath = "/Identity/Account/Login";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                    options.SlidingExpiration = true;
               });
               services.AddSingleton<IEmailSender, EmailSender>();
               services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
          {
               if (env.IsDevelopment())
               {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
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
                    endpoints.MapRazorPages();
               });
          }
     }
}

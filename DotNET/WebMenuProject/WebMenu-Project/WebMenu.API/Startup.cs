using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebMenu.Data.Data;
using WebMenu.Data.Models;
using WebMenu.Data.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Cors;
using System.Globalization;
using WebMenu.Data.Misc;
using WebMenu.Data.Managers;
using Serilog;

namespace WebMenu.API
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
               services.AddScoped<ICompanyManager,CompanyManager>();
               services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
               services.AddDbContext<APIContext>();
               #region Options and Configuration

               services.AddCors(options =>
               {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
               });

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

               services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

               services.AddDistributedMemoryCache();

               // Register the Swagger generator, defining 1 or more Swagger documents
               services.AddSwaggerGen(c =>
               {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web Menu API", Version = "v1", Description = "WM API" });
               });

               services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                 .AddViewLocalization(
                     LanguageViewLocationExpanderFormat.Suffix,
                     opts => { opts.ResourcesPath = "Resources"; })
                 .AddDataAnnotationsLocalization();

               services.Configure<RequestLocalizationOptions>(
                 opts =>
                 {
                      var supportedCultures = new List<CultureInfo>
                     {
                new CultureInfo("en-GB"),
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("tr-TR"),
                new CultureInfo("tr"),
                     };

                      opts.DefaultRequestCulture = new RequestCulture("tr-TR");
             // Formatting numbers, dates, etc.
             opts.SupportedCultures = supportedCultures;
             // UI strings that we have localized.
             opts.SupportedUICultures = supportedCultures;
                 });
               #endregion
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
                    app.UseHsts();
               }
               app.UseSerilogRequestLogging();
               app.UseAuthentication();
               app.UseStaticFiles();
               //app.UseHttpsRedirection();
               app.UseCors("CorsPolicy");
               app.UseSwagger();
               app.UseRouting();
               app.UseSwaggerUI(c =>
               {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WM API V1");
                    c.RoutePrefix = "swagger";
               });
               app.UseAuthorization();

               var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
               app.UseRequestLocalization(options.Value);
               app.UseEndpoints(endpoints =>
               {
                    endpoints.MapControllers();
               });
          }
     }
}

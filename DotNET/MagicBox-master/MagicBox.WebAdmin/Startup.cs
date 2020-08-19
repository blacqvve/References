using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicBox.Data.Managers;
using MagicBox.Data.Misc;
using MagicBox.Data.Data;
using MagicBox.Data.Models;
using MagicBox.Data.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MagicBox.WebAdmin
{
  public class Startup
  {
    private IConfigurationRoot _appSettings;
    private IConfiguration _globalSettings;
    public Startup(IHostingEnvironment env)
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
      #region Scopes

      DataContext.ConnectionString = _globalSettings.GetConnectionString("MySQL");
      services.AddIdentity<User, IdentityRole>()
          .AddDefaultUI(Microsoft.AspNetCore.Identity.UI.UIFramework.Bootstrap4)
          .AddErrorDescriber<CustomIdentityErrorDescriber>()
          .AddEntityFrameworkStores<DataContext>();
      services.AddDbContext<DataContext>();

      services.AddSingleton(_appSettings);
      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddScoped<IPlayerManager, PlayerManager>();
      services.AddScoped<ICouponManager, CouponManager>();

      #endregion

      #region Options and Configuration

      services.AddCors(options =>
      {
        options.AddPolicy("AllowAnyOrigin",
                  builder => builder
                  .AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
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
      #endregion
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });


      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

      app.UseAuthentication();
      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}

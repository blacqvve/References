using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MagicBox.Data.Managers;
using MagicBox.Data.Misc;
using MagicBox.Data.Data;
using MagicBox.Data.Models;
using MagicBox.Data.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MagicBox.API
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
        c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Discount Box API", Version = "v1", Description = "DB API" });
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        .AddJsonOptions(options =>
        {
          options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
          options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        })
        .AddViewLocalization(
            LanguageViewLocationExpanderFormat.Suffix,
            opts => { opts.ResourcesPath = "Resources"; })
        .AddDataAnnotationsLocalization();

      services.Configure<MvcOptions>(options =>
      {
        options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAnyOrigin"));
      });

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
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseAuthentication();
      app.UseStaticFiles();

      //app.UseHttpsRedirection();

      app.UseCors(x => x
         .AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());
      using (var scope = serviceProvider.CreateScope())
      {
        var task = Task.Run(async () =>
        {
          await Seed.AddCouponCodes(
            scope.ServiceProvider.GetService<DataContext>(),
            scope.ServiceProvider.GetService<UserManager<User>>(),
            scope.ServiceProvider.GetService<RoleManager<IdentityRole>>());
        });
        task.Wait();
      }

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DC API V1");
        c.RoutePrefix = "swagger";
      });

      var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
      app.UseRequestLocalization(options.Value);

      app.UseMvc();


    }
  }
}

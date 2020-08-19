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
using CastonFactory.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CastonFactory.Data.Data;
using CastonFactory.Data.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CastonFactory.Data.Managers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ReflectionIT.Mvc.Paging;
using Serilog;
using Microsoft.AspNetCore.Identity.UI.Services;
using CastonFactory.Services;
using System.Text;

namespace CastonFactory
{
     public class Startup
     {
          public Startup(IConfiguration configuration)
          {
               Configuration = configuration;
          }

          public IConfiguration Configuration { get; }

          // This method gets called by the runtime. Use this method to add services to the container.
          public void ConfigureServices(IServiceCollection services)
          {
               Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
               services.AddLocalization(options =>
               {
                    // Resource (kaynak) dosyalarýmýzý ana dizin altýnda “Resources” klasorü içerisinde tutacaðýmýzý belirtiyoruz.
                    options.ResourcesPath = "Resources";
               });
               services.AddDbContext<DataContext>(options =>
                   options.UseMySql(
                       Configuration.GetConnectionString("DefaultConnection")));
               services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
                   .AddEntityFrameworkStores<DataContext>().AddErrorDescriber<LocalizedIdentityErrorDescriber>();
               services.AddControllersWithViews()
     .AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
 );
               services.AddRazorPages();
               services.Configure<IdentityOptions>(options =>
               {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;

                    // User settings
                    options.User.RequireUniqueEmail = false;
                    
               });
               services.AddScoped<IContentManager, ContentManager>();
               services.AddScoped<IHelpers, Helpers>();
               // If you want to tweak Identity cookies, they're no longer part of IdentityOptions.
               services.ConfigureApplicationCookie(options => options.LoginPath = "/Identity/Account/Login");

               // If you don't want the cookie to be automatically authenticated and assigned to HttpContext.User, 
               // remove the CookieAuthenticationDefaults.AuthenticationScheme parameter passed to AddAuthentication.
               services.AddAuthentication()
                       .AddCookie(options =>
                       {
                            options.LoginPath = "/Identity/Account/Login";
                            options.LogoutPath = "/Identity/Account/Logout";
                            options.ExpireTimeSpan = TimeSpan.FromDays(150);
                       });
               services.AddSession(options =>
               {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.Name = ".CastonFactory.Session";
                    options.Cookie.IsEssential = true;
               });
               services.AddMvc().AddRazorRuntimeCompilation();
               services.AddPaging(options=>
               {
                    options.ViewName = "Bootstrap4";
                   
               });
               services.AddTransient<IEmailSender, EmailSender>();
               services.AddSingleton<ICDNService, CDNService>();
               services.Configure<AuthMessageSenderOptions>(Configuration);
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
          {
               // Bu bölüm UseMvc()’ den önce eklenecektir.
           // Uygulamamýz içerisinde destek vermemizi istediðimiz dilleri tutan bir liste oluþturuyoruz.
               var supportedCultures = new List<CultureInfo>
                    {
                    new CultureInfo("tr-TR"),
                    new CultureInfo("en-US"),
                    };
             
               // SupportedCultures ve SupportedUICultures’a yukarýda oluþturduðumuz dil listesini tanýmlýyoruz.
               // DefaultRequestCulture’a varsayýlan olarak uygulamamýzýn hangi dil ile çalýþmasý gerektiðini tanýmlýyoruz.
               app.UseRequestLocalization(new RequestLocalizationOptions
               {
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures,
                    DefaultRequestCulture = new RequestCulture("tr-TR")
               });
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
              
               app.UseSerilogRequestLogging();
               app.UseHttpsRedirection();
               app.UseStaticFiles();
               app.UseRouting();

               app.UseAuthentication();
               app.UseAuthorization();
               app.UseSession();
               app.UseEndpoints(endpoints =>
               {
                    endpoints.MapControllerRoute(
                     name: "default",
                     defaults: "/Identity/Account/Login",
                     pattern: "{controller=Route}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
               });
          }
     }
}

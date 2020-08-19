using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMenu.Data.Data;
using WebMenu.Data.Settings;

namespace WebMenu.Data
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
          public void ConfigureServices(IServiceCollection services)
          {
               APIContext.ConnectionString = _globalSettings.GetConnectionString("Development");
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
          {
               
               if (env.IsDevelopment())
               {
                    app.UseDeveloperExceptionPage();
               }
               app.Run(async (context) => {
                    await context.Response.WriteAsync("Data Systems Operational");
               });
          }
     }
}

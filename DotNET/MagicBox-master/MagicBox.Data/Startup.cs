using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicBox.Data.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicBox.Data
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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Data.DataContext.ConnectionString = _globalSettings.GetConnectionString("MySQL");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Data systems are operational.");
            });
        }
    }
}

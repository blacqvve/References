using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using No2BackgroundTasks.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using No2BackgroundTasks.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using No2BackgroundTasks.Providers;

namespace No2BackgroundTasks
{
    public class Startup
    {
        private IConfigurationRoot _appSettings;
        public Startup(IHostingEnvironment env)
        {
            _appSettings = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
             .AddJsonFile("appsettings.json")
             .Build();

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<APIContext>();
            services.AddSingleton(_appSettings);
            APIContext.ConnectionString = _appSettings.GetConnectionString("DefaultConnection");

            services.AddLogging();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region snippet1
            services.AddHostedService<TimedHostedService>();
            #endregion

            #region snippet2
            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
            #endregion

            #region snippet3
            //services.AddHostedService<QueuedHostedService>();
            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, APIContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            };

            loggerFactory.AddProvider(new LoggerDatabaseProvider(dbContext));

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }
}

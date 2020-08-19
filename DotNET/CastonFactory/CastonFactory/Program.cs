using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CastonFactory.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting;
using Serilog.Formatting.Compact;

namespace CastonFactory
{
     public class Program
     {
          public static void Main(string[] args)
          {
               Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.Async(x => x.Console())
              .WriteTo.Async(x => x.File("Logs\\ErrorLogs-.txt", rollingInterval: RollingInterval.Day, buffered: false, restrictedToMinimumLevel: LogEventLevel.Error))
               .WriteTo.Async(x => x.File("Logs\\InformationLogs-.txt", rollingInterval: RollingInterval.Day, buffered: false, restrictedToMinimumLevel: LogEventLevel.Information, fileSizeLimitBytes: null))
               .WriteTo.Logger(lc=>lc.Filter.ByIncludingOnly(Matching.FromSource<LoginModel>()).WriteTo.Async(x => x.File("Logs\\LoginLogs-.txt", rollingInterval: RollingInterval.Day, buffered: false, restrictedToMinimumLevel: LogEventLevel.Warning)))
        .CreateLogger();

               try
               {
                    Log.Warning("Starting up");
                    CreateHostBuilder(args).Build().Run();
               }
               catch (Exception ex)
               {
                    Log.Fatal(ex, "Application start-up failed");
               }
               finally
               {
                    Log.CloseAndFlush();
               }
          }

          public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
               .UseSerilog()
                  .ConfigureWebHostDefaults(webBuilder =>
                  {

                       webBuilder.UseStartup<Startup>();
                       webBuilder.UseUrls("http://0.0.0.0:5300");
                  });
     }
}

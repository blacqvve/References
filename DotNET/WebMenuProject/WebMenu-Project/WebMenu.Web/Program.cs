using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace WebMenu.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
               Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Async(x => x.Console())
             .Enrich.FromLogContext()
             .WriteTo.Async(x => x.File(new CompactJsonFormatter(), "Logs\\DataLogs-.json", rollingInterval: RollingInterval.Day, buffered: true))
             .CreateLogger();

               try
               {
                    Log.Information("Starting up");

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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

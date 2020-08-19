using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMenu.Data.Settings
{
     public static class JsonConfiguration
     {

          public static IConfiguration ConfigurationContainer { get; private set; }

          public static IConfiguration CreateConfigurationContainer()
          {
               var environment = string.Empty;
               try
               {
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    if (string.IsNullOrWhiteSpace(environment))
                    {
                         //WE ARE IN PROD !!
                         return ConfigurationContainer = new ConfigurationBuilder()
                             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                             .AddJsonFile($"GlobalSettings/GlobalSettings.json").Build();
                    }

                    return ConfigurationContainer = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile($"GlobalSettings/GlobalSettings.{environment}.json").Build();
               }
               catch (Exception)
               {
                    throw;
               }
          }
     }
}

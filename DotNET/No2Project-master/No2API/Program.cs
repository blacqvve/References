using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace No2API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            //.UseUrls("http://192.168.1.39:5010"); //home
          // .UseUrls("http://192.168.1.26:5011"); // yigit ofis
            // .UseUrls("http://192.168.1.21:5012"); // enes ofis
       .UseUrls("http://localhost:5010");
  }
}

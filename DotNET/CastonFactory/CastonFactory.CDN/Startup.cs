using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CastonFactory.CDN
{
     public class Startup
     {
          public static Secrets secrets;
          public Startup(IConfiguration configuration)
          {
               Configuration = configuration;
          }

          public IConfiguration Configuration { get; }

          // This method gets called by the runtime. Use this method to add services to the container.
          public void ConfigureServices(IServiceCollection services)
          {
               services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement, UnicodeRanges.LatinExtendedA }));
               Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
               services.Configure<Secrets>(Configuration.GetSection("Secrets"));
  

                 services.AddControllers();
               services.Configure<FormOptions>(o => {
                    o.ValueLengthLimit = int.MaxValue;
                    o.MultipartBodyLengthLimit = int.MaxValue;
                    o.MemoryBufferThreshold = int.MaxValue;
               });
          }

          // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
          public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
          {
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
               }
               app.UseStaticFiles();
               app.UseStaticFiles(new StaticFileOptions()
               {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                    RequestPath = new PathString("/Resources")
               });
               app.UseHttpsRedirection();

               app.UseRouting();

               app.UseAuthorization();

               app.UseEndpoints(endpoints =>
               {
                    endpoints.MapControllers();
               });
          }
     }
}

using CastonFactory.Data.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CastonFactory.Services
{
     public interface ICDNService
     {
          public Task<ActionReturn> SendFileToCDN(IFormFile file, string fileName);
     }
     public class CDNService : ICDNService
     {
          private readonly ILogger<CDNService> _logger;
          public CDNService(ILogger<CDNService> logger)
          {
               _logger = logger;
          }
          private HttpClient client;


          private const string BASE_URL = "https://cdn-music.caston.tv/";
          private const string AUTH_SCHEME = "Yigit";
          private const string AUTH_KEY = "A9BAAF55-3B01-4A92-B5BA-65160A3E8E06";



          public async Task<ActionReturn> SendFileToCDN(IFormFile file, string fileName)
          {
               client = new HttpClient();
               client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTH_SCHEME, AUTH_KEY);
               client.BaseAddress = new Uri(BASE_URL);
               try
               {

                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                         data = br.ReadBytes((int)file.OpenReadStream().Length);

                    ByteArrayContent bytes = new ByteArrayContent(data);


                    MultipartFormDataContent multiContent = new MultipartFormDataContent();

                    multiContent.Add(bytes, "file", fileName);

                    client.Timeout = TimeSpan.FromMinutes(30);

                    var result = await client.PostAsync("api/Upload/Upload", multiContent);

                    
                    //201 Created the request has been fulfilled, resulting in the creation of a new resource.
                    _logger.LogWarning("file sent");
                    return ActionReturn.Ok;
               }
               catch (Exception ex)
               {
                    _logger.LogError("file send error",ex.Message);
                    return ActionReturn.ServerError;
               }
               finally
               {
                    client.Dispose();
               }

          }
     }
}

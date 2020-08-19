using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OneSignal.API.Managers
{
     public interface ICreateNotification
     {
          public Task<HttpResponseMessage> SendNotification(string parameters);
     }
     public class CreateNotification:ICreateNotification
     {
          HttpClient client = new HttpClient();


          public async Task<HttpResponseMessage> SendNotification(string parameters)
          {
               client.BaseAddress = new Uri(Configuration.RequestUrl);
               client.DefaultRequestHeaders.Clear();
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Configuration.RestAPIKey);


               var data = new StringContent(parameters, Encoding.UTF8, "application/json");

               var httpResponse = await client.PostAsync("notifications", data);
               return httpResponse;
             
          }
     }
}

using Newtonsoft.Json;
using OneSignal.API.Managers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OneSignal.API
{
     public class Signal
     {
          private readonly ICreateNotification createNotification;
          public Signal()
          {
               createNotification = new CreateNotification();
          }

          public async Task<HttpResponseMessage> CreateNotification(string[] segments,string message,string template=Templates.Manager_Login,string appId=Configuration.ApplicationID)
          {
               var obj = new
               {
                    app_id=appId,
                    contents=new {en=message},
                    included_segments=segments,
                    template_id=template
               };
               var parameter = JsonConvert.SerializeObject(obj);
              var response= await createNotification.SendNotification(parameter);

               if (response.IsSuccessStatusCode)
               {
                    return response;
               }
               else
               {
                    return null;
               }
          }

          public async Task<HttpResponseMessage> SendNotificationToUser(string message,string title,string[] userIds,string appId = Configuration.ApplicationID)
          {
               var obj = new
               {
                    app_id = appId,
                    contents = new { en = message },
                    headings =new {en=title},
                    include_external_user_ids = userIds
               };
               var parameter = JsonConvert.SerializeObject(obj);
               var response = await createNotification.SendNotification(parameter);
               var responseText = await response.Content.ReadAsStringAsync();
               if (response.IsSuccessStatusCode)
               {
                    return response;
               }
               else
               {
                    
                    return null;
               }
          }
     }
}

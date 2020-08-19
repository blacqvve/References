using System;
using System.Collections.Generic;
using System.Text;

namespace CastonFactory.Data.JsonObjects
{
     
     public class UserPassword
     {
          /// <summary>
          /// Combine this data with IWebHostEnvironment.ContentRootPath.
          /// </summary>
          public const string  JSON_PATH= "UserPasswords/UserData.json";


          public string UserName { get; set; }

          public string Password { get; set; }

          public UserPassword(string userName, string password)
          {
               UserName = userName;
               Password = password;
          }
     }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CastonFactory.CDN.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CastonFactory.CDN.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class UploadController : ControllerBase
     {
          private readonly Secrets secrets;
          private readonly IWebHostEnvironment webHost;

          public UploadController(IOptions<Secrets> secrets, IWebHostEnvironment webHost)
          {
               this.secrets = secrets.Value ?? throw new ArgumentException(nameof(Secrets));
               this.webHost = webHost;
          }

          [HttpPost("Upload"), DisableRequestSizeLimit]
          public async Task<IActionResult> UploadFile()
          {
               var auth = Request.Headers["Authorization"];

               if (string.IsNullOrEmpty(auth) || !CheckAuthState(auth))
                    return Unauthorized();

               try
               {
                    var file = Request.Form.Files[0];
                    var folderName = Path.Combine("Resources", "Contents");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                    if (file.Length > 0)
                    {
                         var fileName = file.FileName;
                         
                         var fullPath = Path.Combine(pathToSave, fileName);
                         var dbPath = Path.Combine(folderName, fileName);

                         using (var stream = new FileStream(fullPath, FileMode.Create))
                         {
                              await file.CopyToAsync(stream);
                         }

                         return Ok(new { dbPath });
                    }
                    else
                    {
                         return BadRequest();
                    }
               }
               catch (Exception ex)
               {
                    return StatusCode(500, $"Internal server error: {ex}");
               }
          }

          public bool CheckAuthState(string authKey)
          {
               string authType = "Yigit";
               
               if (!authKey.StartsWith(authType))
                    return false;
               string token = authKey.Substring(authType.Length+1).Trim();

               if (!token.Equals(secrets.AuthToken))
                    return false;

               return true;

          }
     }
}

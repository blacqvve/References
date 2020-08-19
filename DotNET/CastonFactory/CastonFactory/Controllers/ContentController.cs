using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CastonFactory.Data.Constants;
using CastonFactory.Data.Enums;
using CastonFactory.Data.Helper;
using CastonFactory.Data.Managers;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneSignal.API;

namespace CastonFactory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin,Otoriter")]
    public class ContentController : ControllerBase
    {
          private readonly IContentManager contentManager;
          private readonly ILogger<ContentController> logger;
          private readonly UserManager<User> _userManager;
          private readonly SignInManager<User> _signManager;
          private readonly IWebHostEnvironment hostEnvironment;
          private Signal signal;

          public ContentController(IContentManager contentManager, ILogger<ContentController> logger, UserManager<User> userManager, SignInManager<User> signManager, IWebHostEnvironment hostEnvironment)
          {
               this.contentManager = contentManager;
               this.logger = logger;
               _userManager = userManager;
               _signManager = signManager;
               this.hostEnvironment = hostEnvironment;
               signal = new Signal();
          }

          [Produces("application/json")]
          [HttpGet("GetContents")]
          public async Task<IActionResult> GetContents([FromQuery]bool active=true)
          {
               var contents = await contentManager.GetContents(active);
               var obj = contents.Select(x => new { x.User.FullName, x.CreationName, x.IsActive, x.Theme.Name, x.ContentType,x.Id });
               return Ok(obj);
          }

          [Produces("application/json")]
          [HttpGet("Search")]

          public async Task<IActionResult> Search()
          {
               string term = HttpContext.Request.Query["term"].ToString().ToLower();
               var terms = await contentManager.GetContentsFromOwnerName(term);
               var model = terms.GroupBy(x => x.User).Select(group => new { Keyword = group.Key.FullName });
               return Ok(model);
          }

          [HttpGet("Deactivate/{id}")]
          public async Task<IActionResult> Deactivate(Guid id)
          {
               var action = await contentManager.DeactivateContent(id);
               
               if (action!=ActionReturn.Ok)
               {
                    return BadRequest(ActionReturnExtensions.GetActionMessage(action));
               }
               logger.LogInformation("Deactivate Success");
               return Ok();
          }
          [HttpGet("Activate/{id}")]
          public async Task<IActionResult> Activate(Guid id)
          {
               var action = await contentManager.ActivateContent(id);
               
               if (action != ActionReturn.Ok)
               {
                    return BadRequest(ActionReturnExtensions.GetActionMessage(action));
               }
               var content = await contentManager.GetContent(id);
               string title = "İçerik aktivasyonu";
               string message = $"{content.CreationName} isimli içeriğiniz otoriterlere görünen havuza eklenmiştir. Bol şans.";
               var response = await signal.SendNotificationToUser(message, title, new string[] { content.User.Id });
               logger.LogInformation("Activate Success");
               return Ok();
          }

          [HttpGet("MarkAsPaid/{id}")]
          public async Task<IActionResult> ActivateAndAddPool(Guid id)
          {
               var action = await contentManager.MarkAsPaidAsync(id);

               if (action != ActionReturn.Ok)
               {
                    return BadRequest(new { Message = action.GetActionMessage() });
               }
               return Ok();
          }
       
          [AllowAnonymous]
          [HttpGet("SetRole")]
          public async Task<IActionResult> SetRole()
          {
               User user = await _userManager.FindByIdAsync("b1a0c7af-908d-4b8a-b2c6-31ca26e9aaa1");
               var result=await _userManager.RemoveFromRoleAsync(user, RoleConstants.MANAGER);
               if (result.Succeeded)
               {
                    return Ok();
               }
               return BadRequest(result.Errors.ToString());
          }

          [Authorize(Roles = "Admin")]
          [HttpGet("RateContent")]
          public async Task<IActionResult> RateContent(Guid id, float rating)
          {
               var rateAction = await contentManager.RateContent(id, rating);
               if (!rateAction.Equals(ActionReturn.Ok))
               {
                    return BadRequest(new { Message = "Puanlama işlemi başarısız." + rateAction.GetActionMessage() });
               }
               return Ok();
          }
     }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CastonFactory.Data.Constants;
using CastonFactory.Data.Data;
using CastonFactory.Data.Enums;
using CastonFactory.Data.Models;
using CastonFactory.Models.SupportModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using OneSignal.API;
using ReflectionIT.Mvc.Paging;

namespace CastonFactory.Controllers
{
     [Authorize]
     public class SupportController : Controller
     {
          private readonly DataContext context;
          private readonly ILogger<SupportController> logger;
          private readonly UserManager<User> userManager;
          private Signal signal;

          public SupportController(DataContext context, ILogger<SupportController> logger, UserManager<User> userManager)
          {
               this.context = context;
               this.logger = logger;
               this.userManager = userManager;
               signal = new Signal();
          }
          [Authorize(Roles = "Admin")]
          public async Task<IActionResult> Index(int page = 1)
          {
               var requests = await context.SupportRequests.Include(x => x.User).Where(x => x.State != SupportState.Closed).ToListAsync();
               var model = PagingList.Create(requests, PagerConstants.PAGER_PAGE_SIZE, page);
               return View(model);
          }
          [Authorize(Roles = "Admin")]
          public async Task<IActionResult> SupportDetails(Guid id)
          {
               var supportRequest = await context.SupportRequests.Include(x => x.User).Include(x=>x.Content).ThenInclude(x=>x.Data).Include(x=>x.Content.Theme).Include(x=>x.Content.Genre).FirstOrDefaultAsync(x => x.Id.Equals(id));
               SupportDetailsViewModel viewModel = new SupportDetailsViewModel()
               {
                    Content = supportRequest.Content,
                    Request = supportRequest

               };

               if (supportRequest == null)
                    return RedirectToAction(nameof(Index));

               return View(viewModel);
          }

          [Authorize(Roles ="Admin")]
          public async Task<IActionResult> ChangeState(SupportState state,Guid id)
          {
               var supporRequest = await context.SupportRequests.Include(x=>x.User)
                     .FirstOrDefaultAsync(x => x.Id.Equals(id));

               supporRequest.State = state;

               context.SupportRequests.Update(supporRequest);
               await context.SaveChangesAsync();
               string title = "Destek Talebiniz Hakkında";
               string message = $"{supporRequest.Subject} konulu destek talebinizin durumu {supporRequest.State.GetStateText()}";
               var notification = await signal.SendNotificationToUser(message, title, new string[] { supporRequest.User.Id });
               return RedirectToAction(nameof(SupportDetails),new { id=id });
          }

          [Authorize(Roles = "ContentProducer,Admin")]
          public async Task<IActionResult> UserRequests(int page = 1)
          {
               var user = await userManager.GetUserAsync(User);
               var requests = await context.SupportRequests.Include(x => x.User).Where(x => x.User == user).OrderByDescending(x=>x.CreateDate).OrderBy(x=>x.State).ToListAsync();
               var model = PagingList.Create(requests, PagerConstants.PAGER_PAGE_SIZE, page);
               return View(model);
          }

          [Authorize(Roles = "ContentProducer,Admin")]
          public async Task<IActionResult> CreateSupportRequest()
          {
               var user = await userManager.GetUserAsync(User);
               var contents = new List<Content>();
               var dummyContent = new Content()
               {
                    Id = Guid.Empty,
                    CreationName = "Şeçiniz"
               };
               contents.Add(dummyContent);
               contents.AddRange(await context.Contents.Where(x => x.User == user).ToListAsync());
               ViewBag.ContentsSelect = contents;
               return View();
          }
          //TODO:Error controls needed.
          [Authorize(Roles = "ContentProducer,Admin")]
          [HttpPost]
          public async Task<IActionResult> CreateSupportRequest(UserCreateSupportRequestVM viewModel)
          {
               var supportRequest = new SupportRequest();
               if (viewModel == null)
                    return View(viewModel);
               var user = await userManager.GetUserAsync(User);
               if (ModelState.IsValid)
               {
                    supportRequest.CreateDate = DateTime.Now;
                    supportRequest.ClosingDate = default;

                    supportRequest.State = SupportState.Open;

                    supportRequest.Id = Guid.NewGuid();
                    supportRequest.Subject = viewModel.Subject;
                    supportRequest.Message = viewModel.Message;
                    supportRequest.Content = await context.Contents.FirstOrDefaultAsync(x => x.Id == Guid.Parse(viewModel.ContentID));

                    if (user == null)
                         return Unauthorized();

                    supportRequest.User = user;

                    try
                    {
                         await context.AddAsync(supportRequest);
                         await context.SaveChangesAsync();
                         logger.LogInformation($"Support request added {supportRequest.Id}");
                         TempData["Success"] = "Destek talebi başarı ile oluşturuldu";
                         var notification = await signal.CreateNotification(new string[] { "AdminSegment" }, $"{supportRequest.User.FullName} isimli kullanıcı bir destek talebi oluşturdu.", Templates.Support_Request_Created);
                         return RedirectToAction("ContentIndex", "User");
                    }
                    catch (Exception ex)
                    {
                         logger.LogError($"error occured while sending support request {supportRequest.Id}", ex.Message);
                         return View(viewModel);
                    }
               }
               return View(viewModel);
          }
     }
}

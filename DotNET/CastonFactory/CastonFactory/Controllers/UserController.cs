using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CastonFactory.Data;
using CastonFactory.Data.Constants;
using CastonFactory.Data.Data;
using CastonFactory.Data.Enums;
using CastonFactory.Data.Managers;
using CastonFactory.Data.Models;
using CastonFactory.Models;
using CastonFactory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneSignal.API;
using ReflectionIT.Mvc.Paging;

namespace CastonFactory.Controllers
{


     [Authorize(Roles = "ContentProducer,Admin")]
     public class UserController : Controller
     {
          private readonly DataContext _context;

          private readonly ILogger<UserController> _logger;
          private readonly IContentManager contentManager;
          private readonly IWebHostEnvironment hosting;
          private readonly SignInManager<User> _signInManager;
          private readonly IHelpers helpers;
          private readonly UserManager<User> _userManager;
          private readonly ICDNService cdnService;
          public UserController(UserManager<User> userManager, 
               SignInManager<User> signInManager, 
               ILogger<UserController> logger, 
               DataContext context,
               IContentManager _contentManager, 
               [FromServices] IWebHostEnvironment hosting, 
               IHelpers _helpers,
               ICDNService service)
          {
               _userManager = userManager;
               _context = context;
               _signInManager = signInManager;
               _logger = logger;
               contentManager = _contentManager;
               helpers = _helpers;
               this.hosting = hosting;
               cdnService = service;
          }

          public async Task<IActionResult> ContentIndex(Guid Id, UserContentVM userContentVM)
          {

               var userId = _userManager.GetUserId(User);
               var userContents = await _context.Contents.Include(x =>x.Rating).Include(x => x.Theme).Include(x => x.Genre).Include(x => x.User).Include(x => x.Data).Where(x => x.User.Id == userId).ToListAsync();

               return View(userContents);
          }


          public async Task<ActionResult> ContentDetails(Guid Id)
          {
               var content = await contentManager.GetContent(Id, allData: true);
               return View(content);
          }

          public async Task<IActionResult> ContentCreate()
          {

               await FillBags();
               TempData.Clear();
               return View();
          }


          [HttpPost]
          [RequestFormLimits(MultipartBodyLengthLimit = 600000000)]
          [RequestSizeLimit(600000000)]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> ContentCreate(UserContentVM content)
          {
               await FillBags();
               var file = HttpContext.Request.Form.Files[0];
               content.File = file;
               var _content = new Content();

               if (ModelState.IsValid)
               {
                    var namedPath = Path.Combine(Guid.NewGuid().ToString() + Path.GetExtension(content.File.FileName));

                    var user = await _userManager.GetUserAsync(User);
                    _content.User = user;

                    _content.CreationName = content.CreationName;
                    _content.ContentType = content.ContentType;



                    _content.Data = new ContentData
                    {
                         Id = Guid.NewGuid(),
                         FileExtension = Path.GetExtension(Path.GetFileName(namedPath)),
                         Link = namedPath
                    };
                    var cdnAction = await cdnService.SendFileToCDN(content.File,namedPath);
                    if (!cdnAction.Equals(ActionReturn.Ok))
                    {
                         ViewBag.Error = $"Dosyanızın cloud servisine iletiminde bir hata oluştu lütfen konu detaylarını destek talebi olarak iletiniz. Hata mesajı : {cdnAction.GetActionMessage()}";
                         return View(content);
                    }


                    var theme = _context.Themes.FirstOrDefault(x => x.Name == content.Theme);

                    if (theme == null || !String.IsNullOrEmpty(content.NewTheme))
                    {
                         theme = new Theme { UserTheme = false, Name = content.NewTheme, Id = Guid.NewGuid(), CreateDate = DateTime.Now };
                         _context.Themes.Add(theme);

                    }
                    var genre = _context.Genres.FirstOrDefault(x => x.Name == content.Genre);

                    if (genre == null || !String.IsNullOrEmpty(content.NewGenre))
                    {
                         genre = new Genre { UserGenre = false, Name = content.NewGenre, Id = Guid.NewGuid(), CreateDate = DateTime.Now };

                         _context.Genres.Add(genre);

                    }

                    _content.Paid = false;
                    _content.IsActive = false;
                    _content.IsPremium = false;
                    _content.Theme = theme;
                    _content.Genre = genre;
                    var action = await contentManager.CreateContent(_content, false);
                    if (action != ActionReturn.Ok)
                    {
                         ViewBag.Error = action.GetActionMessage();
                         _logger.LogError("Create content error. Error Message={action}", action.GetActionMessage());
                    }
                    _logger.LogInformation("Content created {CreationName}", _content.CreationName);
                    Signal signal = new Signal();
                    var resp = await signal.CreateNotification(new string[] { "AdminSegment" }, $"{_content.User.FullName} isimli kullanıcı {_content.CreationName} isimli içeriği ekledi. {_content.CreateDate}.", Templates.Content_Created);
                    _logger.LogWarning(await resp.Content.ReadAsStringAsync());
                    TempData["ContentCreate"] = "İşlem Başarılı";
                    return RedirectToAction(nameof(ContentIndex));
               }
               return View(content);
          }


          public async Task<IActionResult> ContentEdit(Guid? id)
          {
               if (id == null)
               {
                    return NotFound();
               }

               var content = await _context.Contents.Include(x => x.Theme).Include(x => x.User).Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);

               if (content == null)
               {
                    return NotFound();
               }
               var vm = new UserContentEditVM
               {

                    CreationName = content.CreationName,
                    ContentType = content.ContentType.ToString(),
                    Theme = content.Theme.Name,
                    Genre = content.Genre.Name,
               };
               ViewBag.UserTheme = content.Theme.UserTheme;
               ViewBag.UserGenre = content.Genre.UserGenre;
               await FillBags();
               TempData.Clear();
               return View(vm);
          }


          [HttpPost]
          [RequestFormLimits(MultipartBodyLengthLimit = 600000000)]
          [RequestSizeLimit(600000000)]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> ContentEdit(Guid id, UserContentEditVM vm)
          {
               await FillBags();
               var content = await contentManager.GetContent(id);
               if (id != content.Id)
               {
                    return NotFound();
               }
               var filePath = Path.Combine(hosting.WebRootPath, "Contents");
               if (ModelState.IsValid)
               {

                    try
                    {


                         content.ContentType = vm.ContentType;
                         var theme = _context.Themes.FirstOrDefault(x => x.Name == vm.Theme);
                         if (theme == null || !String.IsNullOrEmpty(vm.NewTheme))
                         {
                              theme = new Theme { UserTheme = false, Name = vm.NewTheme, Id = Guid.NewGuid(), CreateDate = DateTime.Now };
                              _context.Themes.Add(theme);

                         }
                         var genre = _context.Genres.FirstOrDefault(x => x.Name == vm.Genre);
                         if (genre == null || !String.IsNullOrEmpty(vm.NewGenre))
                         {
                              genre = new Genre { UserGenre = false, Name = vm.NewGenre, Id = Guid.NewGuid(), CreateDate = DateTime.Now };
                              _context.Genres.Add(genre);

                         }
                         content.Theme = theme;
                         content.Genre = genre;
                         content.isEdited = true;
                         content.ModifyDate = DateTime.Now;
                         content.IsActive = false;
                         content.IsPremium = false;
                         _context.Update(content);
                         await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                         if (!ContentExists(content.Id))
                         {
                              return NotFound();
                         }
                         else
                         {
                              throw;
                         }
                    }
                    TempData["ContentEdit"] = "İşlem Başarılı";
                    return RedirectToAction(nameof(ContentIndex));
               }
               return View(vm);
          }

          private bool ContentExists(Guid ıd)
          {
               throw new NotImplementedException();
          }




          public async Task<ActionResult> ProfileAsync()
          {
               var user = await _userManager.GetUserAsync(User);
               var userVmData = new UserVM { Name = user.Name, Surname = user.Surname, Email = user.Email, PhoneNumber = user.PhoneNumber };

               return View(userVmData);
          }

          public async Task<ActionResult> ProfileEditAsync()
          {

               var userId = _userManager.GetUserId(User);
               var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
               var userVmData = new UserVM { Name = user.Name, Surname = user.Surname, Email = user.Email, PhoneNumber = user.PhoneNumber };
               return View(userVmData);

          }


          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<ActionResult> ProfileEditAsync(UserVM userVM)
          {
               var userId = _userManager.GetUserId(User);
               var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
               user.Name = userVM.Name;
               user.Surname = userVM.Surname;
               user.PhoneNumber = userVM.PhoneNumber;
               user.Email = userVM.Email;
               _context.Update(user);
               _context.SaveChanges();
                
               _logger.LogInformation("User cupdate profile.");

               ViewData["Success"] = "İşlem Başarılı";

               return RedirectToAction(nameof(ContentIndex));
          }

          public ActionResult UserResetPassword()
          {
               TempData.Clear();
               return View();

          }

          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<ActionResult> UserResetPasswordAsync(UserPasswordVM userPasswordVM)
          {

               var userId = _userManager.GetUserId(User);
               var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
               var oldPassword = await _userManager.CheckPasswordAsync(user, userPasswordVM.OldPassword);
               var userRestPassword = _userManager.ChangePasswordAsync(user, userPasswordVM.OldPassword, userPasswordVM.Password);
               if (oldPassword)
               {
                    if (userRestPassword.Result.Succeeded)
                    {

                         _logger.LogInformation("User change password.");

                         ViewData["Success"] = "İşlem Başarılı";
                         TempData["ResetPassword"] = "İşlem Başarılı";


                         return RedirectToAction(nameof(ContentIndex));
                    }

                    ViewData["Success"] = "İşlem Hatalı";

                    return View();
               }
               ViewData["Success"] = "İşlem Hatalı";

               return View();

          }

          public async Task FillBags()
          {
               var tpl = await helpers.GetViewBagDataAsync(_context);
               ViewBag.Themes = tpl.Item2;
               ViewBag.Genres = tpl.Item1;
               var contentTypes = new Dictionary<string, string>();
               var types = tpl.Item3;
               for (int i = 0; i < types.Length; i++)
               {
                    contentTypes.Add(types[i], types[i]);
               }
               ViewBag.Types = contentTypes;
          }
     }
}

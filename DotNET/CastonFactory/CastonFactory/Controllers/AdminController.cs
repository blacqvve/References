using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CastonFactory.Data.Data;
using CastonFactory.Data.Models;
using CastonFactory.Data.Managers;
using Microsoft.Extensions.Logging;
using CastonFactory.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CastonFactory.Data.Enums;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using CastonFactory.Data;
using Microsoft.AspNetCore.Identity;
using CastonFactory.Data.Constants;
using CastonFactory.Data.JsonObjects;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using CastonFactory.Services;
using OneSignal.API;

namespace CastonFactory.Controllers
{
    [Authorize(Roles = ("Admin"))]
    public class AdminController : Controller
    {
        private readonly DataContext context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AdminController> _logger;
        private readonly IContentManager contentManager;
        private readonly IWebHostEnvironment hosting;
        private readonly IHelpers helpers;
        private readonly ICDNService cdnService;
        private Signal signal;

        public AdminController(ILogger<AdminController> logger, UserManager<User> userManager,
            SignInManager<User> signInManager, DataContext _context, IContentManager _contentManager, [FromServices] IWebHostEnvironment hosting, IHelpers _helpers, ICDNService cdn)
        {
            this.context = _context;
            contentManager = _contentManager;
            helpers = _helpers;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            this.hosting = hosting;
            cdnService = cdn;
            signal = new Signal();
        }


        public async Task<IActionResult> Index(string Genre, string Theme, string[] contentTypes, int page = 1, bool filtered = false, string Keyword = "")
        {
   


            int pageSize = PagerConstants.PAGER_PAGE_SIZE;
            var tpl = await helpers.GetViewBagDataAsync(context);
            ViewBag.Genres = tpl.Item1;
            ViewBag.Themes = tpl.Item2;
            ViewBag.Types = tpl.Item3;
            Genre genreQuery = null;
            Theme themeQuery = null;
            var model = PagingList.Create(await contentManager.GetContents(), pageSize, page);
            var routeDict = new RouteValueDictionary();
            if (!filtered)
            {
                return View(model);
            }
            else
            {
                if (filtered == true && (Genre == ContentConstants.NULL_GENRE && Theme == ContentConstants.NULL_THEME && contentTypes.Length == 0 && string.IsNullOrEmpty(Keyword)))
                {
                    return View(model);
                }
                if (!string.IsNullOrEmpty(Keyword))
                {
                    model.Clear();
                    model = PagingList.Create(await contentManager.GetContentsFromOwnerName(Keyword), pageSize, page);
                    routeDict.Clear();
                    routeDict.Add("Keyword", Keyword);
                    routeDict.Add("filtered", true);
                    model.RouteValue = routeDict;
                    return View(model);
                }
                else
                {
                    genreQuery = await context.Genres.FirstOrDefaultAsync(x => x.Name.ToLower() == Genre.ToLower());
                    themeQuery = await context.Themes.FirstOrDefaultAsync(x => x.Name.ToLower() == Theme.ToLower());
                    if (!string.IsNullOrEmpty(Keyword))
                    {
                        model.Clear();
                        model = await helpers.GetFilteredList(themeQuery, genreQuery, contentTypes, FilterTypes.All, pageSize, page);
                        routeDict.Clear();
                        routeDict.Add("Theme", Theme);
                        routeDict.Add("Genre", Genre);
                        routeDict.Add("contentTypes", contentTypes);
                        routeDict.Add("filtered", true);
                        model.RouteValue = routeDict;
                        return View(model);
                    }
                    else if (genreQuery.Name == ContentConstants.NULL_GENRE && themeQuery.Name == ContentConstants.NULL_THEME)
                    {
                        model.Clear();
                        model = await helpers.GetFilteredList(themeQuery, genreQuery, contentTypes, FilterTypes.Type, pageSize, page);
                        routeDict.Clear();
                        routeDict.Add("Theme", Theme);
                        routeDict.Add("Genre", Genre);
                        routeDict.Add("contentTypes", contentTypes);
                        routeDict.Add("filtered", true);
                        model.RouteValue = routeDict;
                        return View(model);
                    }
                    else
                    {
                        if (genreQuery.Name != ContentConstants.NULL_GENRE)
                        {
                            model.Clear();
                            model = await helpers.GetFilteredList(themeQuery, genreQuery, contentTypes, FilterTypes.Genre, pageSize, page);
                            routeDict.Clear();
                            routeDict.Add("Genre", Genre);
                            routeDict.Add("contentTypes", contentTypes);
                            routeDict.Add("filtered", true);
                            model.RouteValue = routeDict;
                            return View(model);
                        }
                        else
                        {
                            model.Clear();
                            model = await helpers.GetFilteredList(themeQuery, genreQuery, contentTypes, FilterTypes.Theme, pageSize, page);
                            routeDict.Clear();
                            routeDict.Add("Theme", Theme);
                            routeDict.Add("contentTypes", contentTypes);
                            routeDict.Add("filtered", true);
                            model.RouteValue = routeDict;
                            return View(model);
                        }
                    }
                }
            }
        }

        public async Task<IActionResult> DeactivatedContents(string Keyword = "")
        {
            List<Content> contents = new List<Content>();
            if (string.IsNullOrEmpty(Keyword))
            {
                contents = await contentManager.GetContents(false);
                return View(contents);
            }
            else
            {
                contents.Clear();
                contents = await contentManager.GetContentsFromOwnerName(Keyword, false);
                return View(contents);
            }
        }

        public async Task<IActionResult> ActivationWaitingList(string Keyword = "")
        {
            List<Content> contents = new List<Content>();
            if (string.IsNullOrEmpty(Keyword))
            {
                contents = await contentManager.GetContents(false, false);

            }
            else
            {
                contents = await contentManager.GetContentsFromOwnerName(Keyword, false, false);
            }
            return View(contents);
        }
        public async Task<IActionResult> ActivetedUserTheme(Guid id)
        {
            var content = await contentManager.GetContent(id);
            content.Theme.UserTheme = true;
            await context.SaveChangesAsync();
            string message = $"{content.CreationName} isimli içeriğinizin Teması sisteme eklenmesi yönetim ekibimiz tarafından onaylandı.";
            string heading = "Tema Onayı";
            var signalRequest = await signal.SendNotificationToUser(message, heading, new string[] { content.User.Id });
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ActivetedUserGenre(Guid id)
        {
            var content = await contentManager.GetContent(id);
            content.Genre.UserGenre = true;
            await context.SaveChangesAsync();
            string message = $"{content.CreationName} isimli içeriğinizin Türü sisteme eklenmesi yönetim ekibimiz tarafından onaylandı.";
            string heading = "Tür Onayı";
            var signalRequest = await signal.SendNotificationToUser(message, heading, new string[] { content.User.Id });
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditedList(Guid id)
        {
            var content = await context.Contents.Where(x => x.isEdited == true).ToListAsync();
            
            return View(content);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var content = await contentManager.GetContent(id);
            return View(content);
        }

        public async Task<IActionResult> CreateAsync()
        {
            await FillBags();
            return View();
        }


        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 600000000)]
        [RequestSizeLimit(600000000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentVM content)
        {
            var _content = new Content();
            await FillBags();

            if (ModelState.IsValid)
            {

                var namedPath = Path.Combine(Guid.NewGuid().ToString() + Path.GetExtension(content.File.FileName));

                var user = await contentManager.CreateUserForContent(_userManager, hosting, content.CreatorName, content.Contact);
                if (user == null)
                {
                    ViewBag.Error = "User oluşturulurken bir hata oluştu. Servis sağlayıcı ile iletişime geçin";
                    return View(content);
                }

                _content.CreationName = content.CreationName;
                _content.ContentType = content.ContentType;
                _content.Paid = true;
                _content.User = user;
                _content.Data = new ContentData
                {
                    Id = Guid.NewGuid(),
                    FileExtension = Path.GetExtension(Path.GetFileName(namedPath)),
                    Link = namedPath
                };

                var cdnAction = await cdnService.SendFileToCDN(content.File, namedPath);

                if (!cdnAction.Equals(ActionReturn.Ok))
                {
                    ViewBag.Error = $"Dosyanızın cloud servisine iletiminde bir hata oluştu lütfen konu detaylarını destek talebi olarak iletiniz. Hata mesajı : {cdnAction.GetActionMessage()}";
                    return View(content);
                }

                var theme = context.Themes.FirstOrDefault(x => x.Name == content.Theme);

                if (theme == null || !String.IsNullOrEmpty(content.NewTheme))
                {
                    theme = new Theme { UserTheme = true, Name = content.NewTheme, Id = Guid.NewGuid(), CreateDate = DateTime.Now };
                    context.Themes.Add(theme);

                }
                var genre = context.Genres.FirstOrDefault(x => x.Name == content.Genre);

                if (genre == null || !String.IsNullOrEmpty(content.NewGenre))
                {
                    genre = new Genre { UserGenre = true, Name = content.NewGenre, Id = Guid.NewGuid(), CreateDate = DateTime.Now };

                    context.Genres.Add(genre);

                }

                _content.Theme = theme;
                _content.Genre = genre;
                var action = await contentManager.CreateContent(_content);
                if (action != ActionReturn.Ok)
                {
                    ViewBag.Error = action.GetActionMessage();
                    _logger.LogError("Create content error. Error Message={action}", action.GetActionMessage());
                }
                _logger.LogInformation("Content created {CreationName}", _content.CreationName);
                return RedirectToAction(nameof(Index));
            }
            return View(content);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await context.Contents.Include(x => x.Theme).Include(x => x.User).Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);

            if (content == null)
            {
                return NotFound();
            }
            var vm = new EditFileVM
            {

                CreatorName = content.User.FullName,
                CreationName = content.CreationName,
                ContentType = content.ContentType.ToString(),
                Theme = content.Theme.Name,
                Genre = content.Genre.Name,
                Contact = content.User.PhoneNumber,
            };
            await FillBags();
            return View(vm);
        }


        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 600000000)]
        [RequestSizeLimit(600000000)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditFileVM vm)
        {
            await FillBags();
            var content = await contentManager.GetContent(id);

            if (id != content.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (vm.File != null)
                {
                    var namedPath = Path.Combine(Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName));

                    content.Data.Link = namedPath;
                    content.Data.FileExtension = Path.GetExtension(Path.GetFileName(namedPath));

                    var cdnAction = await cdnService.SendFileToCDN(vm.File, namedPath);
                    if (!cdnAction.Equals(ActionReturn.Ok))
                    {
                        ViewBag.Error = $"Dosyanızın cloud servisine iletiminde bir hata oluştu lütfen konu detaylarını destek talebi olarak iletiniz. Hata mesajı : {cdnAction.GetActionMessage()}";
                        return View(content);
                    }
                }
                try
                {

                    content.CreationName = vm.CreationName;
                    content.User.PhoneNumber = vm.Contact;
                    content.ContentType = vm.ContentType;
                    var theme = context.Themes.FirstOrDefault(x => x.Name == vm.Theme);
                    if (theme == null || !String.IsNullOrEmpty(vm.NewTheme))
                    {
                        theme = new Theme { UserTheme = true, Name = vm.NewTheme, Id = Guid.NewGuid(), CreateDate = DateTime.Now };
                        context.Themes.Add(theme);

                    }
                    var genre = context.Genres.FirstOrDefault(x => x.Name == vm.Genre);
                    if (genre == null || !String.IsNullOrEmpty(vm.NewGenre))
                    {
                        genre = new Genre { UserGenre = true, Name = vm.NewGenre, Id = Guid.NewGuid(), CreateDate = DateTime.Now };
                        context.Genres.Add(genre);

                    }
                    if (content.Paid != true)
                    {
                        content.Paid = false;
                    }
                    else
                    {
                        content.Paid = true;
                    }
                    content.Theme = theme;
                    content.Genre = genre;
                    content.ModifyDate = DateTime.Now;

                    context.Update(content);
                    await context.SaveChangesAsync();
                    string message = $"{content.CreationName} isimli içeriğiniz yöneticiler tarafından düzenlenmiştir.";
                    string heading = "Düzenleme bildirimi";
                    var response = await signal.SendNotificationToUser(message, heading, new string[] { content.User.Id });
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
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var content = await context.Contents.FindAsync(id);
            context.Contents.Remove(content);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentExists(Guid id)
        {
            return context.Contents.Any(e => e.Id == id);
        }
        public IActionResult ManagerRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagerRegister(ManagerVM managerVM)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = managerVM.UserName,
                    Email = managerVM.Email,
                    Name = managerVM.Name,
                    PasswordHash = managerVM.Password,
                    Surname = managerVM.Surname
                };
                var result = await _userManager.CreateAsync(user, managerVM.Password);
                if (result.Succeeded)
                {
                    var roleresult = await _userManager.AddToRoleAsync(user, "Otoriter");
                    if (roleresult.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        ViewData["Success"] = "İşlem Başarılı";
                        return RedirectToAction(nameof(Index));
                    }


                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    ViewData["Success"] = null;
                }


            }

            return View(managerVM);
        }


        public async Task<IActionResult> PasswordList()
        {
            var path = Path.Combine(hosting.ContentRootPath, UserPassword.JSON_PATH);
            List<UserPassword> userPasswords = new List<UserPassword>();
            if (System.IO.File.Exists(path))
            {
                userPasswords = JsonConvert.DeserializeObject<List<UserPassword>>(await System.IO.File.ReadAllTextAsync(path));
                return View(userPasswords);
            }
            return View(userPasswords);
        }

        public async Task FillBags()
        {
            var tpl = await helpers.GetViewBagDataAsync(context);
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

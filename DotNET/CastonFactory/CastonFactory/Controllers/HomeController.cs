using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CastonFactory.Models;
using CastonFactory.Data.Data;
using CastonFactory.Data.Helper;
using CastonFactory.Data.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using CastonFactory.Data.Managers;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using CastonFactory.Data;
using CastonFactory.Data.Enums;
using Microsoft.AspNetCore.Http;
using CastonFactory.Data.Constants;

namespace CastonFactory.Controllers
{

    [Authorize(Roles = "Admin,Otoriter")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext context;
        private readonly IContentManager contentManager;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IHelpers helpers;

        public HomeController(ILogger<HomeController> logger, DataContext context, IContentManager contentManager, IHttpContextAccessor contextAccessor, IHelpers helpers)
        {
            _logger = logger;
            this.context = context;
            this.contentManager = contentManager;
            this.contextAccessor = contextAccessor;
            this.helpers = helpers;
        }

        //private async Task seed()
        //{
        //     var contents = await contentManager.GetContents(true);
        //     var genre = new Genre {
        //          Id = Guid.NewGuid(),
        //          Name = Constants.NULL_GENRE,
        //          CreateDate = DateTime.Now
        //     };
        //     context.Genres.Add(genre);
        //     foreach (var item in contents)
        //     {
        //          item.Genre = genre;
        //     }
        //     await context.SaveChangesAsync();
        //}
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
                    if (genreQuery.Name != ContentConstants.NULL_GENRE && themeQuery.Name != ContentConstants.NULL_THEME)
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
        public async Task<IActionResult> Details(Guid id)
        {
            var content = await contentManager.GetContent(id);
            content = await contentManager.IncreaseViewCount(content);

            return View(content);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public async Task<IActionResult> RatingPage()  
        {

            var content = await context.Contents.Include(x => x.Genre).Include(x => x.Theme).Include(x =>x.User).Include(x => x.Rating).Where(x => x.Rating.ContentID != null).ToListAsync();

            var orderlist = content.OrderByDescending(x => x.Rating.ViewCount).Take(15);
    
            
            return View(orderlist);
        }
        public async Task<IActionResult> ManagerRatingPage()
        {

            var content = await context.Contents.Include(x => x.Genre).Include(x => x.Theme).Include(x => x.User).Include(x => x.Rating).Where(x => x.Rating.ContentID != null).ToListAsync();

            var orderlist = content.OrderByDescending(x => x.Rating.ManagerRating).Take(15);


            return View(orderlist);
        }
    }
}

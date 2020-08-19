using CastonFactory.Data.Constants;
using CastonFactory.Data.Data;
using CastonFactory.Data.Enums;
using CastonFactory.Data.Managers;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CastonFactory.Data
{
     public  interface IHelpers
     {
          public Task<Tuple<List<Genre>,List<Theme>,string[]>> GetViewBagDataAsync(DataContext context);
          public Task<PagingList<Content>> GetFilteredList(Theme theme, Genre genre, string[] contentTypes, FilterTypes filterType,int pageSize,int pageIndex);
     }
     public  class Helpers :IHelpers
     {
          private readonly IContentManager contentManager;
          private readonly ILogger<Helpers> logger;

          public Helpers(IContentManager contentManager, ILogger<Helpers> logger)
          {
               this.contentManager = contentManager;
               this.logger = logger;
          }

          public  async Task<Tuple<List<Genre>, List<Theme>, string[]>> GetViewBagDataAsync(DataContext context)
          {
               List<Genre> genreList = new List<Genre>();
               genreList.Add(await context.Genres.FirstOrDefaultAsync(x => x.Name == ContentConstants.NULL_GENRE));
               genreList.AddRange(await context.Genres.Where(x => x.Name != ContentConstants.NULL_GENRE && x.UserGenre != false).OrderBy(x => x.Name).ToListAsync());
               List<Theme> themeList = new List<Theme>();
               themeList.Add(await context.Themes.FirstOrDefaultAsync(x => x.Name == ContentConstants.NULL_THEME));
               themeList.AddRange(await context.Themes.Where(x => x.Name != ContentConstants.NULL_THEME && x.UserTheme != false).OrderBy(x => x.Name).ToListAsync());

               return new Tuple<List<Genre>, List<Theme>, string[]>(genreList, themeList, ContentTypes.GetContentTypes());
          }

          
          public async Task<PagingList<Content>> GetFilteredList(Theme theme, Genre genre, string[] contentTypes, FilterTypes filterType,int pageSize,int pageIndex)
          {
               List<Content> contents = new List<Content>();
               switch (filterType)
               {
                    case FilterTypes.Theme:
                         contents = await contentManager.FilterContents(contentTypes, theme);
                         break;
                    case FilterTypes.Genre:
                         contents = await contentManager.FilterContents(contentTypes, genre);
                         break;
                    case FilterTypes.Type:
                         contents = await contentManager.FilterContents(contentTypes);
                         break;
                    case FilterTypes.All:
                         contents = await contentManager.FilterContents(contentTypes, theme, genre);
                         break;
               }
               return PagingList.Create(contents, pageSize, pageIndex);
          }
     }
}

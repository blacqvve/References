using CastonFactory.Data.Constants;
using CastonFactory.Data.Data;
using CastonFactory.Data.Enums;
using CastonFactory.Data.Helper;
using CastonFactory.Data.JsonObjects;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastonFactory.Data.Managers
{
     public interface IContentManager
     {
          public Task<ActionReturn> CreateContent(Content content, bool active = true);

          public Task<ActionReturn> DeactivateContent(Guid id);
          public Task<ActionReturn> ActivateContent(Guid id);


          public Task<List<Content>> GetContents(bool active=true,bool paid =true, bool allData = true,bool isEdited = false);
       
          public Task<Content> GetContent(Guid id, bool allData = true);

          public Task<List<Content>> GetContentsFromOwnerName(string keyword, bool active = true, bool paid = true);

          public Task<List<Content>> FilterContents(string[] contentTypes, Theme theme);
          public Task<List<Content>> FilterContents(string[] contentTypes, Theme theme, Genre genre);
          public Task<List<Content>> FilterContents(string[] contentTypes, Genre genre);
          public Task<List<Content>> FilterContents(string[] contentTypes);

          public Task<Content> IncreaseViewCount(Content content);

          public Task<ActionReturn> RateContent(Guid id, float rating);

          public Task<ActionReturn> MarkAsPaidAsync(Guid id);
          #region ArrangeData
          ///// <summary>
          ///// arranges current content data to new table for every entry and save. Unsafe!!!
          ///// </summary>
          ///// <returns></returns>
          //public Task<ActionReturn> ArrangeData();
          ///// <summary>
          ///// Creates user for every content and write their information to json.Unsafe!!!!
          ///// </summary>
          ///// <param name="userManager"></param>
          ///// <param name="hosting"></param>
          ///// <returns></returns>
          //public Task<ActionReturn> CreateUsersForEveryContent(UserManager<User> userManager, IWebHostEnvironment hosting); 
          #endregion

          /// <summary>
          /// Creates dummy user for content and saves user information to json data.
          /// </summary>
          /// <param name="userManager"></param>
          /// <param name="hoting"></param>
          /// <param name="creatorName"></param>
          /// <param name="phoneNumber"></param>
          /// <returns></returns>
          public Task<User> CreateUserForContent(UserManager<User> userManager, IWebHostEnvironment hoting, string creatorName, string phoneNumber);

     }
     public class ContentManager : IContentManager
     {
          private readonly DataContext context;
          private readonly ILogger<ContentManager> logger;
          public ContentManager(DataContext _context, ILogger<ContentManager> _logger)
          {
               context = _context;
               logger = _logger;
          }
          public async Task<ActionReturn> ActivateContent(Guid id)
          {
               var content = await context.Contents.Include(x => x.Genre).Include(x => x.Theme).FirstOrDefaultAsync(x => x.Id == id);

               if (content == null)
                    return ActionReturn.NotFound;
               if (content.IsActive)
                    return ActionReturn.Ok;
               if (content.Theme.UserTheme == false || content.Genre.UserGenre == false)
                    return ActionReturn.NotReady;
               try
               {
                    content.IsActive = true;
                    content.ModifyDate = DateTime.Now;
                    content.isEdited = false;
                    context.Contents.Update(content);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"{content.Id} activated");
                    return ActionReturn.Ok;
               }
               catch (Exception e)
               {
                    logger.LogError(e, e.Message);
                    return ActionReturn.ServerError;
               }

          }
          #region Unsafe Methods
          //public async Task<ActionReturn> ArrangeData()
          //{
          //     var contents = context.Contents.ToList();
          //     var modifiedContents = new List<Content>();
          //     foreach (var item in contents)
          //     {
          //          var contentData = new ContentData()
          //          {
          //               Id = Guid.NewGuid(),
          //               ContentId = item.Id,
          //               FileExtension = item.FileExtension,
          //               Link = item.ContentData
          //          };
          //          item.Data = contentData;
          //          modifiedContents.Add(item);
          //          context.ContentDatas.Add(contentData);
          //          logger.LogInformation("Content arrange success for {CreationName}", item.CreationName);
          //     }
          //     try
          //     {
          //          context.Contents.UpdateRange(modifiedContents);
          //          await context.SaveChangesAsync();
          //     }
          //     catch (DbUpdateConcurrencyException ex)
          //     {
          //          Console.WriteLine(ex);
          //          throw;
          //     }
          //     return ActionReturn.Ok;
          //}
          //public async Task<ActionReturn> CreateUsersForEveryContent(UserManager<User> userManager, IWebHostEnvironment hosting)
          //{
          //     var path = Path.Combine(hosting.ContentRootPath, UserPassword.JSON_PATH);
          //     var contents = await context.Contents.Include(x => x.User).ToListAsync();
          //     var modifiedContents = new List<Content>();
          //     var userPasswords = new List<UserPassword>();
          //     userPasswords = await Helpers.GetPasswords(hosting);
          //     foreach (var item in contents)
          //     {
          //          var content = item;
          //          if (content.User == null)
          //          {
          //               string userName = UserHelpers.CreateUserName(content.CreatorName);
          //               User user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
          //               var password = RandomPasswordGenerator.GenerateRandomPassword();
          //               if (user == null)
          //               {
          //                    var createAction = UserHelpers.CreateUser(ref user, content);
          //                    if (createAction != ActionReturn.Ok)
          //                    {
          //                         logger.LogError("Error on filling user data");
          //                         return createAction;
          //                    }
          //                    var result = await userManager.CreateAsync(user, password);
          //                    if (result.Succeeded)
          //                    {
          //                         var roleResult = await userManager.AddToRoleAsync(user, RoleConstants.CONTENTPRODUCER);
          //                         if (roleResult.Succeeded)
          //                         {
          //                              logger.LogWarning("New  user added to role {username} => {rolename} ", user.UserName, RoleConstants.CONTENTPRODUCER);
          //                              var up = new UserPassword(userName, password);
          //                              content.User = user;
          //                              if (userPasswords == null)
          //                              {
          //                                   userPasswords = new List<UserPassword>();
          //                              }
          //                              userPasswords.Add(up);
          //                              logger.LogWarning("New User added to content {UsernName}=>{CreatorName} ID={id}", user.UserName, content.User.UserName, content.Id);
          //                         }
          //                    }
          //               }
          //               else
          //               {

          //                    content.User = user;
          //                    logger.LogWarning("Existing User added to content {UsernName}=>{CreatorName}  ID={id}", user.UserName, content.User.UserName, content.Id);

          //               }

          //          }
          //          modifiedContents.Add(content);
          //     }
          //     // serialize JSON directly to a file
          //     var jsonData = JsonConvert.SerializeObject(userPasswords);
          //     var writeAction = await Helpers.WritePasswordsToFile(hosting, jsonData);
          //     if (writeAction != ActionReturn.Ok)
          //     {
          //          return writeAction;
          //     }
          //     try
          //     {
          //          context.Contents.UpdateRange(modifiedContents);
          //          await context.SaveChangesAsync();
          //     }
          //     catch (Exception ex)
          //     {
          //          Console.WriteLine(ex.Message);
          //          return ActionReturn.ServerError;
          //     }
          //     return ActionReturn.Ok;
          //}
          #endregion
          public async Task<ActionReturn> CreateContent(Content content, bool active = true)
          {
               if (content != null)
               {
                    try
                    {
                         content.Id = Guid.NewGuid();
                         content.IsActive = active;
                         content.IsPremium = false;
                         content.CreateDate = DateTime.Now;
                         content.ModifyDate = DateTime.Now;


                         await context.Contents.AddAsync(content);
                         await context.SaveChangesAsync();
                         logger.LogInformation($"{content.Id} data created");
                         return ActionReturn.Ok;
                    }
                    catch (Exception e)
                    {
                         logger.LogError(e, e.Message);
                         return ActionReturn.ServerError;
                    }
                    //TODO:figure out for returning content data.
               }
               return ActionReturn.NotFound;
          }



          public async Task<ActionReturn> DeactivateContent(Guid id)
          {
               var content = await context.Contents.FirstOrDefaultAsync(x => x.Id == id);

               if (content == null)
                    return ActionReturn.NotFound;
               if (!content.IsActive)
                    return ActionReturn.Ok;

               try
               {
                    content.IsActive = false;
                    content.ModifyDate = DateTime.Now;

                    context.Contents.Update(content);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"{content.Id} activated");
                    return ActionReturn.Ok;
               }
               catch (Exception e)
               {
                    logger.LogError(e, e.Message);
                    return ActionReturn.ServerError;
               }
          }

          public async Task<List<Content>> FilterContents(string[] contentTypes, Theme theme)
          {
               if (theme == null)
                    return new List<Content>();
               var contents = new List<Content>();

               if (contentTypes.Length > 0)
               {
                    for (int i = 0; i < contentTypes.Length; i++)
                    {
                         contents.AddRange(
                              await context.Contents
                              .Where(x => x.ContentType == contentTypes[i] && x.Theme == theme)
                              .Include(x => x.Theme)
                              .ToListAsync()
                              );
                    }
                    contents = contents.Where(x => x.IsActive && x.Paid).ToList();
                    return contents;
               }
               else
               {

                    contents = await context.Contents.Where(x => x.Theme == theme && x.IsActive && x.Paid).ToListAsync();
                    return contents;
               }

          }
          public async Task<List<Content>> FilterContents(string[] contentTypes, Theme theme, Genre genre)
          {
               var contents = new List<Content>();

               if (contentTypes.Length > 0)
               {
                    for (int i = 0; i < contentTypes.Length; i++)
                    {
                         contents.AddRange(
                              await context.Contents
                              .Where(x => x.ContentType == contentTypes[i] && x.Theme == theme && x.Genre == genre)
                              .Include(x => x.Theme)
                              .Include(x => x.Genre)
                              .ToListAsync()
                              );
                    }


                    contents = contents.Where(x => x.IsActive && x.Paid).ToList();
                    return contents;
               }
               else
               {

                    contents = await context.Contents.Where(x => x.Theme == theme && x.Genre == genre && x.IsActive && x.Paid).ToListAsync();
                    return contents;
               }

          }
          public async Task<List<Content>> FilterContents(string[] contentTypes, Genre genre)
          {
               var contents = new List<Content>();

               if (contentTypes.Length > 0)
               {
                    for (int i = 0; i < contentTypes.Length; i++)
                    {
                         contents.AddRange(
                              await context.Contents
                              .Where(x => x.ContentType == contentTypes[i] && x.Genre == genre)
                              .Include(x => x.Theme)
                              .Include(x => x.Genre)
                              .ToListAsync()
                              );
                    }


                    contents = contents.Where(x => x.IsActive && x.Paid).ToList();
                    return contents;
               }
               else
               {

                    contents = await context.Contents.Where(x => x.Genre == genre && x.IsActive && x.Paid).ToListAsync();
                    return contents;
               }

          }

          public async Task<List<Content>> FilterContents(string[] contentTypes)
          {
               var contents = new List<Content>();

               if (contentTypes.Length > 0)
               {
                    for (int i = 0; i < contentTypes.Length; i++)
                    {
                         contents.AddRange(
                              await context.Contents
                              .Where(x => x.ContentType == contentTypes[i])
                              .Include(x => x.Theme)
                              .Include(x => x.Genre)
                              .ToListAsync()
                              );
                    }


                    contents = contents.Where(x => x.IsActive && x.Paid).ToList();
                    return contents;
               }

               return contents;

          }

          public async Task<Content> GetContent(Guid id, bool allData = true)
          {
               if (allData)
               {
                    return await context.Contents
                                    .Include(x => x.Theme)
                                    .Include(x => x.Genre)
                                    .Include(x => x.Data)
                                    .Include(x => x.User)
                                    .Include(x => x.Rating)
                                    .FirstOrDefaultAsync(x => x.Id == id);
               }
               else
               {
                    return await context.Contents
                                    .FirstOrDefaultAsync(x => x.Id == id);
               }
          }

          //TODO:Refactor

          public async Task<List<Content>> GetContents(bool active =true, bool paid=true ,bool allData = true,bool isEdited = false)
          {
               if (allData)
               {
                         return await context.Contents.Where(x => x.IsActive == active && x.Paid==paid && x.isEdited == isEdited)
                                         .Include(x => x.Theme)
                                         .Include(x => x.Genre)
                                         .Include(x => x.Data)
                                         .Include(x => x.User)
                              .OrderBy(x => x.CreateDate)
                              .ToListAsync();
               }
               else
               {
                    return await context.Contents.Where(x => x.IsActive == active && x.Paid == paid)
                         .OrderBy(x => x.CreateDate)
                         .ToListAsync();
               }
          }

          public async Task<List<Content>> GetContentsFromOwnerName(string keyword, bool active = true, bool _paid = true)
          {
               if (!string.IsNullOrEmpty(keyword))
               {
                    return await context.Contents
                         .Where(x => (x.User.Name + " " + x.User.Surname).ToLower().Contains(keyword.ToLower()) && x.IsActive == active && x.Paid == _paid)
                                    .Include(x => x.Theme)
                                    .Include(x => x.Genre)
                                    .Include(x => x.Data)
                                    .Include(x => x.User)
                         .ToListAsync();
               }
               return new List<Content>();
          }

          public async Task<User> CreateUserForContent(UserManager<User> userManager, IWebHostEnvironment hosting, string creatorName, string phoneNumber)
          {
               var userPasswords = await Helpers.GetPasswords(hosting);

               var userName = UserHelpers.CreateUserName(creatorName);
               User user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
               if (user == null)
               {
                    var createUserAction = UserHelpers.CreateUser(ref user, creatorName, phoneNumber);
                    if (createUserAction != ActionReturn.Ok)
                    {
                         return user;
                    }
                    var password = RandomPasswordGenerator.GenerateRandomPassword();

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                         var roleResult = await userManager.AddToRoleAsync(user, RoleConstants.CONTENTPRODUCER);
                         if (roleResult.Succeeded)
                         {
                              var userPassword = new UserPassword(user.UserName, password);
                              if (userPasswords == null)
                              {
                                   userPasswords = new List<UserPassword>();
                              }
                              userPasswords.Add(userPassword);

                              var writeAction = await Helpers.WritePasswordsToFile(hosting, JsonConvert.SerializeObject(userPasswords));
                              if (writeAction != ActionReturn.Ok)
                              {
                                   logger.LogError("Password Write Error. UserName= {userName} Password = {password}", user.UserName, password);
                              }

                              return user;
                         }
                         else
                         {
                              return null;
                         }
                    }
                    else
                    {
                         return null;
                    }
               }
               return user;
          }

          public async Task<ActionReturn> MarkAsPaidAsync(Guid id)
          {
               var content = await GetContent(id, false);

               if (content == null)
                    return ActionReturn.NotFound;
               if (content.Paid)
                    return ActionReturn.Ok;

               try
               {
                    content.Paid = true;

                    context.Update(content);
                    await context.SaveChangesAsync();
                    return ActionReturn.Ok;
               }
               catch (Exception e)
               {
                    logger.LogError($"An error occured while marking content as paid. Content : {content.Id} \n Error Message :{e.Message}");
                    return ActionReturn.ServerError;
               }
          }

          public async Task<Content> IncreaseViewCount(Content content)
          {
               if (content.Rating == null)
               {
                    ContentRating rating = new ContentRating { Id = Guid.NewGuid(), ContentID = content.Id, ViewCount = 0 };
                    content.Rating = rating;
                    context.ContentRatings.Add(rating);
               }
               content.Rating.ViewCount++;

               try
               {

                    context.Update(content);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"View increased. Content:{content.Id}");
                    return content;

               }
               catch (Exception ex)
               {
                    logger.LogError(ex, $"Error while view count increase. Content{content.Id}");
                    return content;
               }
          }

          public async Task<ActionReturn> RateContent(Guid id, float rating)
          {
               var content = await context.Contents.Include(x => x.Rating).FirstOrDefaultAsync(x => x.Id == id);

               if (content == null)
                    return ActionReturn.NotFound;

               if (content.Rating==null)
               {
                    ContentRating newRating = new ContentRating() { ContentID = content.Id, ViewCount = 0, Id = Guid.NewGuid() };
                    content.Rating = newRating;
                    context.ContentRatings.Add(newRating);
               }

               content.Rating.ManagerRating = (int)rating*2;

               try
               {
                    context.Update(content);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"Content rating changed. Content: {content.Id}");
                    return ActionReturn.Ok;
               }
               catch (Exception ex)
               {
                    logger.LogError(ex, $"Content rating change error. Content: {content.Id}");
                    return ActionReturn.ServerError;
               }
          }
     }
}

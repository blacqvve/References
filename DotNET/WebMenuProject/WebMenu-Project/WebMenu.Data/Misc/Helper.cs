using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMenu.Data.Data;
using WebMenu.Data.Misc.Enums;
using WebMenu.Data.Models;

namespace WebMenu.Data.Misc
{
     public static class Helper
     {
          /// <summary>
          /// Returns error message in string form 
          /// </summary>
          /// <param name="error"></param>
          /// <returns></returns>
          public static string GetErrorMessage(ErrorReturns error)
          {
               switch (error)
               {
                    case ErrorReturns.NotFound:
                         return "İşlem sonucu veri bulunamadı.";
                    case ErrorReturns.ServerError:
                         return "Sunucuda bir hata oluştu lütfen daha sonra tekrar deneyiniz.";
                    case ErrorReturns.TypeMismatch:
                         return "Gönderilen veri tipi uyuşmazlığı.";
                    case ErrorReturns.AccesDenied:
                         return "Bu bölüme erişim izniniz yok.";
                    case ErrorReturns.Ok:
                         return "İşlem başarılı.";
                    default:
                         return "";
               }

          }
          public static async Task<bool> SeedDataToDatabase(APIContext context, ILogger logger,int count)
          {
               try
               {
                    string[] categoryTypes = new string[] { "Ana Yemekler", "Kahvaltılıklar", "Sıcak İçecekler", "Soğuk İçecekler" };   

                    Random rnd = new Random();
                    var e = Enum.GetValues(typeof(CompanyType));

                    for (int i = 0; i < count; i++)
                    {
                         List<MenuItem> menuItems = new List<MenuItem>();
                         for (int j = 0; j < rnd.Next(0,20); j++)
                         {
                              var menuItem = new MenuItem()
                              {
                                   Active = true,
                                   CreateDate = DateTime.Now,
                                   ItemDescription = "Test company test menu item description " + j,
                                   ItemName = "Test company test menu item " + j,
                                   ItemId = Guid.NewGuid(),
                                   ModifyDate = DateTime.Now,
                                   ItemPrice = (decimal)rnd.NextDouble(),
                                   Pictures = null,

                              };
                              menuItems.Add(menuItem);
                         }
                         var categories = new List<Category>();
                         for (int l = 0; l < rnd.Next(0, 6); l++)
                         {
                            
                              string cat = categoryTypes[rnd.Next(0, categoryTypes.Length)];
                              var category = new Category()
                              {
                                   Active = true,
                                   CategoryDescription = "Test category " + cat,
                                   CategoryId = Guid.NewGuid(),
                                   CategoryImage = null,
                                   CreateDate = DateTime.Now,
                                   ModifyDate=DateTime.Now,
                                   CategoryName = cat,
                                   MenuItems = menuItems
                              };
                              categories.Add(category);
                         }
                         var company = new Company()
                         {
                              CompanyId = Guid.NewGuid(),
                              CompanyName = "TestCompany " + i,
                              CompanyAdress = "Test company adress information " + i,
                              CompanyLogo = new Picture
                              {
                                   PictureId = Guid.NewGuid(),
                                   URL = " https://i.redd.it/f7hdtrt41jd21.png",
                                   CreateDate = DateTime.Now
                              },
                              CompanyType = (CompanyType)e.GetValue(rnd.Next(e.Length)),
                              User = null,
                              CreateDate = DateTime.Now,
                              ModifyDate = DateTime.Now,
                              Subscription = rnd.Next(0, 2) == 1 ? true : false,
                              Menu = new Menu
                              {
                                   MenuId = Guid.NewGuid(),
                                   MenuName = "Test menu " + i,
                                   Categories=categories
                                   
                              }
                         };
                       await context.Companies.AddAsync(company);
                    }
               }
               catch (Exception e)
               {
                    logger.LogWarning(e, e.Message);
                    return false;
               }
               await context.SaveChangesAsync();
               return  true;
          }
     }
}

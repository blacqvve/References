using CastonFactory.Data.Data;
using CastonFactory.Data.Enums;
using CastonFactory.Data.JsonObjects;
using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastonFactory.Data.Helper
{
     public static class RandomPasswordGenerator
     {


          public static string GenerateRandomPassword(PasswordOptions opts = null)
          {
               if (opts == null) opts = new PasswordOptions()
               {
                    RequireDigit = true,
                    RequiredLength = 8,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = true,
                    RequireLowercase = true,
               };

               string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

               Random rand = new Random(Environment.TickCount);
               List<char> chars = new List<char>();

               if (opts.RequireUppercase)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[0][rand.Next(0, randomChars[0].Length)]);

               if (opts.RequireLowercase)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[1][rand.Next(0, randomChars[1].Length)]);

               if (opts.RequireDigit)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[2][rand.Next(0, randomChars[2].Length)]);

               if (opts.RequireNonAlphanumeric)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[3][rand.Next(0, randomChars[3].Length)]);

               for (int i = chars.Count; i < opts.RequiredLength
                   || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
               {
                    string rcs = randomChars[rand.Next(0, randomChars.Length)];
                    chars.Insert(rand.Next(0, chars.Count),
                                  rcs[rand.Next(0, rcs.Length)]);
               }

               return new string(chars.ToArray());
          }

     
     }
     public static class StringTools
     {
          public static string RemoveWhitespace(string text)
          {
               return new string(text.ToCharArray().Where(x => !Char.IsWhiteSpace(x)).ToArray());
          }
          public static string RemoveDiacritics(string text)
          {
               Encoding srcEncoding = Encoding.UTF8;
               Encoding destEncoding = Encoding.GetEncoding(1252); // Latin alphabet

               text = destEncoding.GetString(Encoding.Convert(srcEncoding, destEncoding, srcEncoding.GetBytes(text)));

               string normalizedString = text.Normalize(NormalizationForm.FormD);
               StringBuilder result = new StringBuilder();

               for (int i = 0; i < normalizedString.Length; i++)
               {
                    if (!CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]).Equals(UnicodeCategory.NonSpacingMark))
                    {
                         result.Append(normalizedString[i]);
                    }
               }

               return result.ToString();
          }
     }
     public static class UserHelpers
     {
          public static ActionReturn CreateUser(ref User user, string creatorName, string phoneNumber)
          {
               var nameArray = creatorName.Split(' ');
               var name = "";
               var userName = CreateUserName(creatorName);

               if (nameArray.Length >= 3)
               {
                    for (int i = 0; i < nameArray.Length - 1; i++)
                    {
                         name += nameArray[i] + " ";
                    }
               }
               else
               {
                    name = nameArray[0];
               }
               user = new User()
               {
                    Name = name,
                    Surname = nameArray[nameArray.Length - 1],
                    PhoneNumber = StringTools.RemoveWhitespace(phoneNumber),
                    UserName = userName,
                    Email = userName + "@gmail.com"
               };

               return ActionReturn.Ok;
          }
          public static string CreateUserName(string creatorName)
          {
               var nameArray = creatorName.Split(' ');
               string userName = StringTools.RemoveWhitespace(nameArray[0]) + StringTools.RemoveWhitespace(nameArray[1]);
               userName = StringTools.RemoveDiacritics(userName).ToLower();
               return userName;
          }
     }
     public static class Helpers
     {
          #region comment
          ///// <summary>
          ///// Seed database for dummy entries. Unsafe!!!
          ///// </summary>
          ///// <param name="context"></param>
          ///// <param name="count"></param>
          //public static void Seed(DataContext context, int count)
          //{
          //     Random rnd = new Random();
          //     var contentTypes = ContentTypes.GetContentTypes();
          //     string[] themeNames = new string[5] { "Aşk", "Para", "Tatil", "Sex", "Uzay" };
          //     var themeList = new List<Theme>();
          //     for (int i = 0; i < themeNames.Length; i++)
          //     {
          //          var theme = new Theme
          //          {
          //               Id = Guid.NewGuid(),
          //               CreateDate = DateTime.Now,
          //               Name = themeNames[i]
          //          };
          //          themeList.Add(theme);
          //     }
          //     var contentList = new List<Content>();
          //     for (int i = 0; i < count; i++)
          //     {
          //          var content = new Content
          //          {
          //               ContentData = "testData" + i,
          //               ContentType = contentTypes[rnd.Next(0, contentTypes.Length - 1)],
          //               CreateDate = DateTime.Now,
          //               ModifyDate = DateTime.Now,
          //               CreationName = "Test Data Name " + i,
          //               CreatorName = "Test" + i + " User" + i,
          //               Id = Guid.NewGuid(),
          //               IsActive = true,
          //               IsPremium = false,
          //               Theme = themeList[rnd.Next(0, themeList.Count)]
          //          };
          //          contentList.Add(content);
          //     }
          //     context.Themes.AddRange(themeList);
          //     context.Contents.AddRange(contentList);
          //     context.SaveChanges();
          //} 
          #endregion

          public async static Task<List<UserPassword>> GetPasswords(IWebHostEnvironment hosting)
          {
               var path = Path.Combine(hosting.ContentRootPath, UserPassword.JSON_PATH);
               List<UserPassword> userPasswords = new List<UserPassword>();
               if (File.Exists(path))
               {
                    userPasswords = JsonConvert.DeserializeObject<List<UserPassword>>(await File.ReadAllTextAsync(path));
                    return userPasswords;
               }

               return userPasswords;

          }

          public async static Task<ActionReturn> WritePasswordsToFile(IWebHostEnvironment hosting,string jsonData)
          {
               var path = Path.Combine(hosting.ContentRootPath, UserPassword.JSON_PATH);

               if (File.Exists(path))
               {
                    try
                    {
                         await File.WriteAllTextAsync(path, jsonData);
                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine(ex.Message);
                         return ActionReturn.ServerError;
                    }
                    return ActionReturn.Ok;
               }

               return ActionReturn.ServerError;

          }
     }

}

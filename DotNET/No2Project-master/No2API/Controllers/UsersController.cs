using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using No2API.Data;
using No2API.Helpers;
using No2API.Entities.Models;
using static No2API.Data.Utils;
using No2API.Entities.Data;


namespace No2API.Controllers
{

     [Route("api/v1/users")]
     [ApiController]
     [EnableCors("AllowAnyOrigin")]
     //[Authorize(Roles = "Admin")]
     public class UsersController : ControllerBase
     {
          private readonly APIContext context;
          private readonly AppSettings appSettings;
          private readonly IUtils utils;
          private readonly IHostingEnvironment hosting;

          public UsersController(APIContext _context, IOptions<AppSettings> _appSettings, IUtils _utils, IHostingEnvironment _hosting)
          {
               utils = _utils;
               context = _context;
               appSettings = _appSettings.Value;
               hosting = _hosting;
          }
          #region Work

          [HttpGet]
          [Route("GetAppliedWorks")]
          [Authorize]
          public IActionResult GetAppliedWorks()
          {
               try
               {
                    var u_Id = new Guid(User.Identity.Name);
                    var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
                    if (user == null)
                         return Unauthorized();
                    var works = context.WorkApplications.Where(x => x.Applicant.Id == user.Id).ToList().Select(x => new { x.ApplyDate, x.Work, x.Applicant });
                    if (works == null)
                         return BadRequest(new { message = "Başvuru bulunamadı." });
                    return Ok(works);
               }
               catch (Exception e)
               {
                    Console.WriteLine(e.Message);
                    return BadRequest(new { message = "İşlem Başarısız" });

               }
          }



          #endregion
          // GET api/v1/GetUsers
          [HttpGet("GetAllUsers")]
          [Authorize(Roles = "Developer, Admin, Manager")]
          public IActionResult GetAllUsers(int? take)
          {
               var _take = take ?? context.Users.Count();
               var list = context.Users
                   .Include(x => x.Pictures)
                   .Include(x => x.Videos)
                   .Include(x => x.CouponCodes)
                   .OrderByDescending(x => x.CreatedAt)
                   .Take(_take)
                   .Where(x => x.HasPaid == true)
                   .ToList();
               list.ForEach(x => x.Pictures = x.Pictures.OrderByDescending(m => m.CreatedAt).ToList());
               return Ok(list);
          }

          // GET api/v1/GetUsers
          [HttpGet("GetUsers")]
          [Authorize(Roles = "Developer, Admin, Manager")]
          public IActionResult GetUsers(int dataLength = 0)
          {
               var list = context.Users
                   .Where(x => x.HasPaid == true)
                   .Include(x => x.Pictures)
                   .Include(x => x.Videos)
                   .OrderByDescending(x => x.CreatedAt)
                   .Skip(dataLength)
                   .Take(20)
                   .ToList();
               list.ForEach(x =>
               {
                    x.Password = null;
                    x.Token = null;
                    x.Pictures = x.Pictures.OrderByDescending(m => m.CreatedAt).ToList();
               });
               return Ok(list);
          }

          // POST api/v1/GetUsers
          [HttpPost("GetUsers")]
          [Authorize(Roles = "Developer, Admin, Manager")]
          public IActionResult GetUsers([FromBody]JObject info, [FromQuery]int dataLength = 0)
          {
               var take = 20;
               var list = utils.FilterUsers(info);
               list = list
                   .Where(x => x.HasPaid == true)
                   .Skip(dataLength)
                   .Take(take)
                   .ToList();
               list.ForEach(x => x.Password = null);
               return Ok(list);
          }

          // GET api/v1/GetUser/5
          [Authorize(Roles = "Developer, Admin, Manager")]
          [HttpGet("GetUser/{id}")]
          public IActionResult GetUser(Guid id)
          {
               try
               {
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    if (user == null)
                         return NotFound();
                    user.Views += 1;
                    context.Update(user);
                    context.SaveChanges();
                    return Ok(user);
               }
               catch (ArgumentNullException e)
               {
                    return BadRequest(e.Message);
               }
          }

          //GET api/v1/GetUser
          [Authorize]
          [HttpGet("GetUser")]
          public IActionResult GetUser()
          {
               try
               {
                    Guid id = Guid.Parse(User.Identity.Name);
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    if (user == null)
                         return NotFound();
                    user.Password = null;
                    return Ok(user);
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          //GET api/v1/GetUserInfo/1
          [Authorize(Roles = "Developer, Admin, Manager")]
          [HttpGet("GetUserInfo/{id}")]
          public IActionResult GetUserInfo(Guid id)
          {
               try
               {
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    var userinfo = context.Userinfos.Where(x => x.User.Id == id).ToList();
                    var infos = new JObject();
                    foreach (var item in userinfo)
                    {
                         infos.Add(item.Info.Name, Utils.IsValidJson(item.Value) ? JToken.Parse(item.Value) : item.Value);
                    }
                    if (user == null || userinfo == null)
                         return NotFound();
                    return Ok(infos);
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          //GET api/v1/GetUserInfo
          [Authorize]
          [HttpGet("GetUserInfo")]
          public IActionResult GetUserInfo()
          {
               try
               {
                    Guid id = Guid.Parse(User.Identity.Name);
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    var userinfo = context.Userinfos.Where(x => x.User.Id == id).ToList();
                    var infos = new JObject();
                    foreach (var item in userinfo)
                    {
                         infos.Add(item.Info.Name, Utils.IsValidJson(item.Value) ? JToken.Parse(item.Value) : item.Value);
                    }
                    if (user == null || userinfo == null)
                         return NotFound();
                    return Ok(infos);
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          [Authorize(Roles = "Developer, Admin, Manager")]
          [HttpGet("GetUsersByRole")]
          public IActionResult GetUsersByRole(string role)
          {
               if (string.IsNullOrEmpty(role))
                    return BadRequest("İstenilen rol bilgisi boş bırakılamaz.");
               var users = context.Users.Where(x => x.Role == role).ToList();
               return Ok();
          }

          //GET api/v1/GetUserInfo
          [Authorize]
          [HttpGet("GetUserPictures")]
          public IActionResult GetUserPictures()
          {
               try
               {
                    var id = new Guid(User.Identity.Name);
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    var pics = context.Pictures.Where(x => x.User.Id == id).OrderByDescending(x => x.CreatedAt).ToList();
                    return Ok(new { pics, path = "content/images" });
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          //GET api/v1/GetUserInfo
          [Authorize]
          [HttpGet("GetUserVideos")]
          public IActionResult GetUserVideos()
          {
               try
               {
                    var id = new Guid(User.Identity.Name);
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    var videos = context.Videos.Where(x => x.User.Id == id).ToList();
                    return Ok(new { videos = videos, path = "content/videos" });
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          //GET api/v1/GetUserInfo
          [Authorize(Roles = "Developer, Admin, Manager")]
          [HttpGet("GetUserPictures/{id}")]
          public IActionResult GetUserPictures(Guid id)
          {
               try
               {
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    var pics = context.Pictures.Where(x => x.User.Id == id).OrderByDescending(x => x.CreatedAt).ToList();
                    return Ok(new { pics, path = "content/images" });
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          //GET api/v1/GetUserInfo
          [Authorize(Roles = "Developer, Admin, Manager")]
          [HttpGet("GetUserVideos/{id}")]
          public IActionResult GetUserVideos(Guid id)
          {
               try
               {
                    if (Guid.Empty == id || id == null)
                         return BadRequest("Kullanıcı bulunamadı.");
                    var user = context.Users.FirstOrDefault(x => x.Id == id);
                    var videos = context.Pictures.Where(x => x.User.Id == id).OrderByDescending(x => x.CreatedAt).ToList();
                    return Ok(new { videos = videos, path = "content/videos" });
               }
               catch (Exception e)
               {
                    return BadRequest(e.Message);
               }
          }

          // POST api/v1/CreateUser
          [Authorize(Roles = "Developer, Admin")]
          [HttpPost("CreateUser")]
          public IActionResult CreateUser([FromBody]User user)
          {
               if (utils.UserExists(user.Email))
                    return BadRequest("Bu e-mail zaten kullanılıyor.");
               if (user.Email == null || user.Password == null)
                    return BadRequest("Email ve şifre alanları boş bırakılamaz.");
               user.Password = Utils.HashString.GetMD5(user.Password);
               user.CreatedAt = DateTime.Now;
               user.Id = Guid.NewGuid();
               var token = utils.Authenticate(user);
               user.Token = token;
               var code = utils.GenerateEmailConfirmationCode(user);
               user.MailConfirmationToken = code;
               context.Users.Add(user);

               //string scheme = HttpContext.Request.Scheme;
               var url = appSettings.EmailConfirmationUrl + "?token=" + user.MailConfirmationToken;
               if (user.Role == RoleConfig.Salesman)
               {
                    context.CouponCodes.Add(new CouponCode
                    {
                         User = user,
                         CreatedAt = DateTime.Now,
                         Code = utils.GenerateCouponCode(),
                         ExpiresAt = DateTime.Now.Add(TimeSpan.FromDays(1080)),
                         Active = true,
                         Discount = 0
                    });
               }

               context.SaveChanges();
               utils.SendConfirmationMail(user, url);
               return Ok(new { token });
          }

          [AllowAnonymous]
          [HttpPost("RegisterUser")]
          public IActionResult RegisterUser([FromBody]User user, string coupon)
          {
               if (utils.UserExists(user.Email))
                    return BadRequest("Bu e-mail zaten kullanılıyor.");
               if (user.Email == null || user.Password == null)
                    return BadRequest("Email ve şifre alanları boş bırakılamaz.");

               user.Password = HashString.GetMD5(user.Password);
               user.CreatedAt = DateTime.Now;
               user.Role = RoleConfig.User;
               user.Id = Guid.NewGuid();
               var token = utils.Authenticate(user);
               user.Token = token;
               var code = utils.GenerateEmailConfirmationCode(user);
               user.MailConfirmationToken = code;
               context.Users.Add(user);

               if (!string.IsNullOrEmpty(coupon))
               {
                    var couponcode = context.CouponCodes.FirstOrDefault(x => x.Code == coupon);
                    if (couponcode == null)
                         return BadRequest("Geçersiz referans kodu.");
                    var referrer = context.Users.FirstOrDefault(x => x.Id == couponcode.User.Id);
                    context.Add(new CouponUser() { User = user, CouponCode = couponcode });
                    user.Referrer = referrer;
               }

               context.SaveChanges();
               //string scheme = HttpContext.Request.Scheme;
               var url = appSettings.EmailConfirmationUrl + "?token=" + user.MailConfirmationToken;
               utils.SendConfirmationMail(user, url);
               return Ok(new { token });
          }

          [AllowAnonymous]
          [HttpPost("TryRegisterUser")]
          public IActionResult TryRegisterUser([FromBody] User user, string coupon)
          {
               if (utils.UserExists(user.Email))
                    return BadRequest("Bu e-mail zaten kullanılıyor.");
               if (user.Email == null || user.Password == null)
                    return BadRequest("Email ve şifre alanları boş bırakılamaz.");
               if (!string.IsNullOrEmpty(coupon))
               {
                    var couponcode = context.CouponCodes.FirstOrDefault(x => x.Code == coupon);
                    if (couponcode == null)
                         return BadRequest("Geçersiz referans kodu.");
               }
               return Ok();
          }

          // PUT api/users/5
          [Authorize]
          [HttpPost("UpdateUser/{id}")]
          public IActionResult UpdateUser(Guid id, [FromBody] User user)
          {
               if (Guid.Empty == id || id == null)
                    return BadRequest("Kullanıcı bulunamadı.");
               var userRole = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
               if (userRole != "Admin" && id != new Guid(User.Identity.Name))
                    return Unauthorized();
               var _user = context.Users.FirstOrDefault(x => x.Id == id);
               if (_user == null)
                    return NotFound("Kullanıcı bulunamadı.");
               if (!string.IsNullOrEmpty(user.Password))
               {
                    _user.Password = HashString.GetMD5(user.Password);
               }
               if (!string.IsNullOrEmpty(user.Phone))
               {
                    _user.Phone = user.Phone;
               }
               if (!string.IsNullOrEmpty(user.Email))
               {
                    _user.Email = user.Email;
               }
               if (user.DateOfBirth != null)
               {
                    _user.DateOfBirth = user.DateOfBirth;
               }
               context.Users.Update(_user);
               context.SaveChanges();
               return Ok(new { message = "User updated." });
          }

          // DELETE api/users/5
          [Authorize(Roles = "Developer, Admin")]
          [HttpDelete("DeleteUser/{id}")]
          public IActionResult Delete(Guid id)
          {
               if (Guid.Empty == id || id == null)
                    return BadRequest("Kullanıcı bulunamadı.");
               var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
               if (user == null)
                    return BadRequest("Kullanıcı bulunamadı.");
               context.Users.Remove(user);
               context.SaveChanges();
               return Ok(new { message = "User deleted." });
          }

          [AllowAnonymous]
          [HttpPost("Login")]
          public IActionResult Login([FromBody]User user)
          {
               var u = utils.UserExists(user.Email) ? utils.GetUserFromName(user.Email) : null;
               if (u == null)
                    return BadRequest("Kullanıcı bulunamadı.");
               if (string.IsNullOrEmpty(user.Password) || u.Password != Utils.HashString.GetMD5(user.Password))
                    return BadRequest("Yanlış kullanıcı adı veya şifre.");
               var token = utils.Authenticate(u);
               u.Token = token;
               context.Update(u);
               context.SaveChanges();
               return Ok(u);
          }

          //[AllowAnonymous]
          [Authorize]
          [HttpGet("Login")]
          public IActionResult Login()
          {
               var u = context.Users.FirstOrDefault(x => x.Id == new Guid(User.Identity.Name));
               if (u == null)
                    return BadRequest();
               var token = utils.Authenticate(u);
               u.Token = token;
               context.Update(u);
               context.SaveChanges();
               return Ok(u);
          }

          //if user id null user_id = User.Identifier.Name

          [Authorize]
          [HttpPost("AddUserInfo")]
          public IActionResult AddUserInfo([FromBody]JObject info)
          {
               var u_id = new Guid(User.Identity.Name);
               foreach (var item in info)
               {
                    if (!context.Infos.Any(x => x.Name == item.Key))
                    {
                         context.Infos.Add(new Info()
                         {
                              Name = item.Key,
                              CreatedAt = DateTime.Now,
                         });
                         context.SaveChanges();
                    }
                    var prop = context.Infos.FirstOrDefault(x => x.Name == item.Key);
                    if (!context.Userinfos.Any(x => x.Info.Id == prop.Id && x.User.Id == u_id))
                    {
                         context.Userinfos.Add(new Userinfo()
                         {
                              User = context.Users.FirstOrDefault(x => x.Id == u_id),
                              Info = prop,
                              Value = item.Value.ToString(),
                              CreatedAt = DateTime.Now
                         });
                         context.SaveChanges();
                    }
                    else
                    {
                         var user_info = context.Userinfos.FirstOrDefault(x => x.Info.Id == prop.Id && x.User.Id == u_id);
                         user_info.Value = item.Value.ToString();
                         context.Update(user_info);
                         context.SaveChanges();
                    }
               }
               return Ok();
          }

          [Authorize(Roles = "Developer, Admin")]
          [HttpPost("AddUserInfo/{id}")]
          public IActionResult AddUserInfo(Guid id, [FromBody]JObject info)
          {
               var u_id = id;
               foreach (var item in info)
               {
                    if (!context.Infos.Any(x => x.Name == item.Key))
                    {
                         context.Infos.Add(new Info()
                         {
                              Name = item.Key,
                              CreatedAt = DateTime.Now,
                         });
                         context.SaveChanges();
                    }
                    var prop = context.Infos.FirstOrDefault(x => x.Name == item.Key);
                    if (!context.Userinfos.Any(x => x.Info.Id == prop.Id && x.User.Id == u_id))
                    {
                         context.Userinfos.Add(new Userinfo()
                         {
                              User = context.Users.FirstOrDefault(x => x.Id == u_id),
                              Info = prop,
                              Value = item.Value.ToString(),
                              CreatedAt = DateTime.Now
                         });
                         context.SaveChanges();
                    }
                    else
                    {
                         var user_info = context.Userinfos.FirstOrDefault(x => x.Info.Id == prop.Id && x.User.Id == u_id);
                         user_info.Value = item.Value.ToString();
                         context.Update(user_info);
                         context.SaveChanges();
                    }
               }
               return Ok();
          }


          [HttpPost]
          [Route("UploadImage")]
          [Authorize]
          public IActionResult UploadImage(IFormFile file, string type, Guid? id)
          {
               var u_id = new Guid(User.Identity.Name);
               var user = context.Users.FirstOrDefault(x => x.Id == u_id);
               var typeExists = context.Pictures.Where(x => x.User.Id == u_id && x.Type == type).Any();
               if (typeExists && type != "feed")
               {
                    foreach (var item in context.Pictures.Where(x => x.User.Id == u_id && x.Type == type).ToList())
                    {
                         utils.RemovePicture(item);
                         context.Remove(item);
                    }
                    context.SaveChanges();
               }
               var pic = utils.AddPicture(user, file, type);
               if (id != null && id != Guid.Empty)
               {
                    if (type == "feed")
                    {
                         var feed = context.Feeds.FirstOrDefault(x => x.Id == id);
                         feed.Picture = pic;
                         context.Update(feed);
                    }
                    else if (type == "manager")
                    {
                         pic.User = context.Users.FirstOrDefault(x => x.Id == id);
                         context.Update(pic);
                    }
               }
               context.SaveChanges();
               return Ok();
          }

          [HttpPost]
          [Route("UploadVideo")]
          [Authorize]
          public IActionResult UploadVideo(IFormFile file, string type)
          {
               var u_id = new Guid(User.Identity.Name);
               var user = context.Users.FirstOrDefault(x => x.Id == u_id);
               var typeExists = context.Videos.Where(x => x.User.Id == u_id && x.Type == type).Any();
               //var fileExists = context.Videos.Any(x => x.Name == file.FileName);
               //if (fileExists)
               //    return BadRequest("Bu dosya zaten kayıtlı.");
               if (typeExists)
               {
                    foreach (var item in context.Videos.Where(x => x.User.Id == u_id && x.Type == type).ToList())
                    {
                         utils.RemoveVideo(item);
                         context.Remove(item);
                    }
                    context.SaveChanges();
               }
               utils.AddVideo(user, file, type);
               return Ok();
          }

          [HttpGet("GetWatchlist")]
          [Authorize(Roles = "Developer, Admin")]
          public IActionResult GetWatchlist()
          {
               var u_id = new Guid(User.Identity.Name);
               var list = new List<User>();
               foreach (var watchlist in context.Watchlists.Where(x => x.Watcher.Id == u_id).ToList())
               {
                    list.Add(watchlist.Watched);
               }
               return Ok(list);
          }

          [HttpPost]
          [Route("AddToWatchlist")]
          [Authorize(Roles = "Developer, Admin")]
          public IActionResult AddToWatchlist(Guid watchedId)
          {
               var u_id = new Guid(User.Identity.Name);
               var watcher = context.Users.FirstOrDefault(x => x.Id == u_id);
               var watched = context.Users.FirstOrDefault(x => x.Id == watchedId);
               var exists = context.Watchlists.Any(x => x.Watcher.Id == u_id && x.Watched.Id == watched.Id);
               if (exists)
                    return BadRequest("Bu kullanıcı zaten listenizde.");
               var watchlist = new Watchlist()
               {
                    CreatedAt = DateTime.Now,
                    Watcher = watcher,
                    Watched = watched
               };

               context.Watchlists.Add(watchlist);
               context.SaveChanges();
               return Ok();
          }

          [HttpPost]
          [Route("RemoveFromWatchlist")]
          [Authorize(Roles = "Developer, Admin")]
          public IActionResult RemoveFromWatchlist(Guid watchedId)
          {
               var u_id = new Guid(User.Identity.Name);
               context.Watchlists.Remove(context.Watchlists.FirstOrDefault(x => x.Watcher.Id == u_id && x.Watcher.Id == watchedId));
               context.SaveChanges();
               return Ok();
          }

          [HttpPost]
          [Route("CreateSale")]
          [Authorize(Roles = "Developer, Admin, Salesman")]
          public IActionResult CreateSale([FromBody] Sale sale)
          {
               context.Add(sale);
               context.SaveChanges();
               return Ok();
          }

          [HttpGet]
          [Route("GetCoupons/{id}/{take?}")]
          [Authorize(Roles = "Developer, Admin, Salesman")]
          public IActionResult GetCoupons(Guid id, int take)
          {
               var coupons = context.CouponCodes
                   .Include(x => x.User)
                   .Take(take)
                   .Where(x => x.User.Id == id)
                   .ToList();
               if (coupons == null)
                    return NotFound();
               return Ok(coupons);
          }

          [HttpPost]
          [Route("ResetPasswordRequest")]
          [AllowAnonymous]
          public IActionResult ResetPasswordRequest([FromBody]User info)
          {
               var user = utils.GetUserFromName(info.Email);
               if (user == null)
                    return NotFound();
               var token = utils.GenerateEmailConfirmationCode(user);
               //string scheme = HttpContext.Request.Scheme;
               var url = appSettings.ResetPasswordUrl + "?token=" + token;
               user.ResetPasswordToken = token;
               context.Update(user);
               context.SaveChanges();
               utils.SendPasswordResetEmail(user, url);
               return Ok();
          }

          [HttpPost]
          [Route("PurchaseSubscription")]
          [Authorize]
          public IActionResult PurchaseSubscription([FromQuery] string subtype, [FromQuery] string OrderId)
          {
               var id = new Guid(User.Identity.Name);
               if (Guid.Empty == id || id == null)
                    return BadRequest("Geçersiz kullanıcı.");
               var user = context.Users.FirstOrDefault(x => x.Id == id);
               if (user == null)
                    return NotFound("Kullanıcı bulunamadı.");
               var subs = Constants.SubTimes;
               user.HasPaid = true;
               context.Orders.Add(new Order() { OrderId = OrderId, User = user });
               user.ActivationDate = DateTime.Now;
               var expireTime = subs[subtype];
               if (subs[subtype] != null)
               {
                    var expirationDate = DateTime.Now.Add(expireTime);
                    if (expirationDate > user.ExpirationDate)
                    {
                         user.ExpirationDate = expirationDate;
                    }
               }
               if (!string.IsNullOrEmpty(subtype))
                    user.Subscription = subtype;
               context.Update(user);
               context.SaveChanges();
               return Ok(subtype);
          }

          [HttpPost]
          [Route("ConfirmSubscription")]
          [Authorize]
          public IActionResult ConfirmSubscription([FromQuery] string subtype, [FromQuery] string OrderId)
          {
               var id = new Guid(User.Identity.Name);
               if (Guid.Empty == id || id == null)
                    return BadRequest("Geçersiz kullanıcı.");
               var user = context.Users.FirstOrDefault(x => x.Id == id);
               if (user == null)
                    return NotFound("Kullanıcı bulunamadı.");
               var subs = Constants.SubTimes;
               user.HasPaid = false;
               var order = context.Orders.FirstOrDefault(x => x.OrderId == OrderId);
               if (order == null)
               {
                    var userOrders = context.Orders.Where(x => x.User.Id == user.Id).ToList();
                    context.RemoveRange(userOrders);
                    order = new Order() { OrderId = OrderId, User = user };
                    context.Orders.Add(order);
               }
               if (order.User.Id == id)
               {
                    user.HasPaid = true;
                    var expireTime = subs[subtype];
                    if (subs[subtype] != null)
                    {
                         var expirationDate = DateTime.Now.Add(expireTime);
                         if (expirationDate > user.ExpirationDate)
                         {
                              user.ExpirationDate = expirationDate;
                         }
                    }
                    context.Update(user);
                    context.SaveChanges();
                    return Ok(subtype);
               }
               context.Update(user);
               context.SaveChanges();
               return BadRequest("Hatalı.");
          }

          [HttpPost]
          [Route("ConfirmSubscriptionWithID")]
          [Authorize(Roles = "Developer")]
          public IActionResult ConfirmSubscriptionWithID([FromQuery] string subtype, [FromQuery] string OrderId, [FromQuery] Guid id)
          {
               if (Guid.Empty == id || id == null)
                    return BadRequest("Geçersiz kullanıcı.");
               var user = context.Users.FirstOrDefault(x => x.Id == id);
               if (user == null)
                    return NotFound("Kullanıcı bulunamadı.");
               var subs = Constants.SubTimes;
               user.HasPaid = false;
               var order = context.Orders.FirstOrDefault(x => x.OrderId == OrderId);
               if (order == null)
               {
                    var userOrders = context.Orders.Where(x => x.User.Id == user.Id).ToList();
                    context.RemoveRange(userOrders);
                    order = new Order() { OrderId = OrderId, User = user };
                    context.Orders.Add(order);
               }
               if (order.User.Id == id)
               {
                    user.HasPaid = true;
                    var expireTime = subs[subtype];
                    if (subs[subtype] != null)
                    {
                         var expirationDate = DateTime.Now.Add(expireTime);
                         if (expirationDate > user.ExpirationDate)
                         {
                              user.ExpirationDate = expirationDate;
                         }
                    }
                    context.Update(user);
                    context.SaveChanges();
                    return Ok(subtype);
               }
               context.Update(user);
               context.SaveChanges();
               return BadRequest("Hatalı.");
          }

          [HttpPost]
          [Route("RevokeSubscription")]
          [Authorize]
          public IActionResult RevokeSubscription()
          {
               var id = new Guid(User.Identity.Name);
               if (Guid.Empty == id || id == null)
                    return BadRequest("Geçersiz kullanıcı.");
               var user = context.Users.FirstOrDefault(x => x.Id == id);
               if (user == null)
                    return NotFound("Kullanıcı bulunamadı.");
               if (!user.Orders.Any(x => x.OrderId.Contains("MANUALORDER_")))
               {
                    user.HasPaid = false;
                    user.ActivationDate = DateTime.MinValue;
                    user.ExpirationDate = DateTime.MinValue;
                    var userOrders = context.Orders.Where(x => x.User.Id == user.Id).ToList();
                    context.RemoveRange(userOrders);
                    context.Update(user);
                    context.SaveChanges();
               }
               return Ok();
          }

          [HttpGet]
          [Route("HasUserPaid")]
          [Authorize]
          public IActionResult HasUserPaid()
          {
               var id = new Guid(User.Identity.Name);
               if (Guid.Empty == id || id == null)
                    return BadRequest("Geçersiz kullanıcı.");
               var user = context.Users.FirstOrDefault(x => x.Id == id);
               if (user == null)
                    return NotFound("Kullanıcı bulunamadı.");
               return Ok(user.HasPaid);
          }

          [HttpPost]
          [Route("dummy/{id}")]
          [Authorize(Roles = "Developer, Admin")]
          public IActionResult dummy(int id, [FromBody]JObject info)
          {
               var i = id;
               var r = new Random();
               var user = new User();
               user.Name = "Kullanici" + i;
               user.Surname = "Kullanici Soyisim" + i;
               user.Email = "kullanici" + i + "@mail.com";
               user.Password = "Kullanici" + i;
               user.Role = "User";
               user.Phone = "05" + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString() + r.Next(0, 9).ToString();
               user.Gender = r.Next(1) == 1 ? "Male" : "Female";
               DateTime now = DateTime.Now;
               DateTime saveNow
                = new DateTime(now.Year - r.Next(15, 40), now.Month, now.Day, now.Hour, now.Minute, 0);
               user.DateOfBirth = saveNow;
               user.Password = Utils.HashString.GetMD5(user.Password);
               user.CreatedAt = DateTime.Now;
               user.Id = Guid.NewGuid();
               var token = utils.Authenticate(user);
               user.Token = token;
               var code = utils.GenerateEmailConfirmationCode(user);
               user.MailConfirmationToken = code;
               context.Users.Add(user);
               context.SaveChanges();
               //string scheme = HttpContext.Request.Scheme;
               var url = appSettings.EmailConfirmationUrl + "?token=" + user.MailConfirmationToken;
               var u_id = user.Id;
               foreach (var item in info)
               {
                    if (!context.Infos.Any(x => x.Name == item.Key))
                    {
                         context.Infos.Add(new Info()
                         {
                              Name = item.Key,
                              CreatedAt = DateTime.Now,
                         });
                         context.SaveChanges();
                    }
                    var prop = context.Infos.FirstOrDefault(x => x.Name == item.Key);
                    if (!context.Userinfos.Any(x => x.Info.Id == prop.Id && x.User.Id == u_id))
                    {
                         context.Userinfos.Add(new Userinfo()
                         {
                              User = context.Users.FirstOrDefault(x => x.Id == u_id),
                              Info = prop,
                              Value = item.Value.ToString(),
                              CreatedAt = DateTime.Now
                         });
                         context.SaveChanges();
                    }
                    else
                    {
                         var user_info = context.Userinfos.FirstOrDefault(x => x.Info.Id == prop.Id && x.User.Id == u_id);
                         user_info.Value = item.Value.ToString();
                         context.Update(user_info);
                         context.SaveChanges();
                    }
               }
               return Ok();
          }

     }
}

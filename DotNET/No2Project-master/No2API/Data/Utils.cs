using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using No2API.Helpers;
using No2API.Entities.Models;
using Powells.CouponCode;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using No2API.Entities.Data;

namespace No2API.Data
{
    public static class RoleConfig
    {
        public static readonly string Founder = "Developer";
        public static readonly string Admin = "Admin";
        public static readonly string Salesman = "Salesman";
        public static readonly string User = "User";
    }

    public static class Constants
    {
        public static Dictionary<string, TimeSpan> SubTimes = new Dictionary<string, TimeSpan>()
            {
                {"monthly", TimeSpan.FromDays(30)},
                {"sixmonthly", TimeSpan.FromDays(182.5)},
                {"yearly", TimeSpan.FromDays(365)},

            };
    }

    public interface IUtils
    {
        List<User> FilterUsers(JObject info);
        bool UserExists(string email);
        User GetUserFromName(string email);
        string Authenticate(User user);
        void SendEmail(string email, string subject, string message);
        void SendConfirmationMail(User user, string url);
        void SendPasswordResetEmail(User user, string url);
        string GenerateEmailConfirmationCode(User user);
        string GenerateCouponCode();
        Picture AddPicture(User user, IFormFile file, string type);
        Video AddVideo(User user, IFormFile file, string type);
        void RemovePicture(Picture pic);
        void RemoveVideo(Video video);
        bool RevokeSubscription(User user);
    }

    public class Utils : IUtils
    {
        private APIContext context;
        private readonly AppSettings appSettings;
        private readonly EmailSettings emailSettings;
        private readonly IHostingEnvironment hosting;
        public Utils(IOptions<AppSettings> _appSettings, APIContext c, IOptions<EmailSettings> _emailSettings, IHostingEnvironment _hosting)
        {
            context = c;
            hosting = _hosting;
            appSettings = _appSettings.Value;
            emailSettings = _emailSettings.Value;
        }

        private List<User> RemoveUnlisted(List<User> users, JObject info)
        {
            var list = users.ToList();
            foreach (var item in (info["user_info"] as JObject))
            {
                if (item.Value.Type == JTokenType.Array)
                {
                    foreach (var criteria in item.Value)
                    {
                        foreach (var user in users)
                        {
                            var userinfo = context.Userinfos.Where(x => x.Info.Name == item.Key && x.User.Id == user.Id).ToList();
                            if (!userinfo.Any(x => x.Info.Name == item.Key
                            && (JsonConvert.DeserializeObject(x.Value) as JArray)
                                .Any(k => k["name"].ToString() == criteria["name"].ToString() && k["checked"].ToString() == criteria["checked"].ToString())))
                                list.Remove(user);
                        }
                    }
                }
                else
                {

                    foreach (var user in users)
                    {
                        var userinfo = context.Userinfos.Where(x => x.Info.Name == item.Key && x.User.Id == user.Id).ToList();
                        if (!userinfo.Any(x => x.Info.Name == item.Key && x.Value.Contains(item.Value.ToString().Trim())))
                            list.Remove(user);
                    }
                }
            }
            return list;
        }

        public List<User> FilterUsers(JObject info)
        {
            var list = new List<User>();
            foreach (var item in (info["user_info"] as JObject))
            {
                if (item.Value.Type == JTokenType.Array)
                {
                    foreach (var criteria in item.Value)
                    {
                        var users = context.Userinfos
                            .Where(x => x.Info.Name == item.Key
                            && (JsonConvert.DeserializeObject(x.Value) as JArray)
                                .Any(k => k["name"].ToString() == criteria["name"].ToString() && k["checked"].ToString() == criteria["checked"].ToString()))
                            .Select(x => x.User)
                            .ToList();

                        foreach (var user in users)
                        {
                            if (!list.Any(x => x.Id == user.Id))
                                list.Add(user);
                        }
                    }
                }
                else
                {
                    var users = context.Userinfos.Where(x => x.Info.Name == item.Key && x.Value.Contains(item.Value.ToString().Trim())).Select(x => x.User).ToList();
                    foreach (var user in users)
                    {
                        if (!list.Any(x => x.Id == user.Id))
                            list.Add(user);
                    }
                }
            }
            list = RemoveUnlisted(list, info);
            list = FilterCustom(list, info);
            return list;
        }

        private List<User> FilterCustom(List<User> users, JObject info)
        {
            var list = users.ToList();
            var _users = list.ToList();
            var userinfos = new List<Userinfo>();
            foreach (var user in list)
            {
                userinfos.AddRange(context.Userinfos.Where(x => x.User.Id == user.Id).ToList());
            }
            foreach (var info_name in new string[] { "height", "weight" })
            {
                foreach (var comparable in new string[] { "Min", "Max" })
                {
                    var compare_str = info_name + comparable;
                    if (info[compare_str] != null && !String.IsNullOrEmpty(info[compare_str].ToString()))
                    {
                        var compare = Convert.ToInt32(info[compare_str]);
                        userinfos = userinfos
                        .Where(x => x.Info.Name == comparable
                            && x.Value != null
                            && Convert.ToInt32(x.Value) >= compare
                        ).ToList();
                        _users = userinfos.Select(x => x.User).ToList(); 
                    }
                }
            }           
            if (info["ageMin"] != null && !String.IsNullOrEmpty(info["ageMin"].ToString()))
            {
                var ageMin = Convert.ToInt32(info["ageMin"]);
                _users = _users.Where(x => DateTime.Now.Year - x.DateOfBirth.Year >= ageMin).ToList();               
            }

            if (info["ageMax"] != null && !String.IsNullOrEmpty(info["ageMax"].ToString()))
            {
                var ageMax = Convert.ToInt32(info["ageMax"]);
                _users = _users.Where(x => DateTime.Now.Year - x.DateOfBirth.Year <= ageMax).ToList();
            }

            return _users;
        }

        public bool UserExists(string email)
        {
            return context.Users.Any(x => x.Email == email);
        }

        public User GetUserFromName(string email)
        {
            return context.Users.FirstOrDefault(x => x.Email == email);
        }

        public string Authenticate(User user)
        {
            //var user = context.Users.SingleOrDefault(x => x.Username == _user.Username && x.Password == _user.Password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.Now.AddYears(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            //user.Password = null;
            return user.Token;
        }

        public string GenerateEmailConfirmationCode(User user)
        {
            return HashString.GetMD5(user.Email + DateTime.Now);
        }

        public void SendEmail(string email, string subject, string message)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(emailSettings.Sender, emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(emailSettings.Sender, emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = emailSettings.MailServer,
                    EnableSsl = false,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

        public Picture AddPicture(User user, IFormFile file, string type)
        {
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName) + ".jpg";
            var dir = Path.Combine(hosting.WebRootPath, "content/images");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var pic = new Picture();
            pic.Name = fileName;
            pic.User = user;
            pic.Type = type;
            pic.CreatedAt = DateTime.Now;

            context.Pictures.Add(pic);
            context.SaveChanges();
            return pic;
        }

        public Video AddVideo(User user, IFormFile file, string type)
        {
            var extension = System.IO.Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
            var dir = Path.Combine(hosting.WebRootPath, "content/videos");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var video = new Video();
            video.Name = fileName;
            video.User = user;
            video.Type = type;
            video.CreatedAt = DateTime.Now;

            context.Videos.Add(video);
            context.SaveChanges();
            return video;
        }

        public void RemovePicture(Picture file)
        {
            var dir = Path.Combine(hosting.WebRootPath, "content/images");
            var fileName = file.Name;

            if (Directory.Exists(dir))
            {
                if (File.Exists(Path.Combine(dir, fileName)))
                {
                    File.Delete(Path.Combine(dir, fileName));
                }
            }
        }

        public void RemoveVideo(Video file)
        {
            var dir = Path.Combine(hosting.WebRootPath, "content/videos");
            var fileName = file.Name;

            if (Directory.Exists(dir))
            {
                if (File.Exists(Path.Combine(dir, fileName)))
                {
                    File.Delete(Path.Combine(dir, fileName));
                }
            }
        }

        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void SendConfirmationMail(User user, string url)
        {
            var callbackUrl = url;
            SendEmail(user.Email, "[CASTON] Lütfen E-mail Adresinizi Doğrulayın",
                    $"<strong>Merhaba {user.Name + " " + user.Surname}.</strong><br/>" +
                    $"Lütfen <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bu linke</a> tıklayarak email adresinizi doğrulayın.");
        }

        public void SendPasswordResetEmail(User user, string url)
        {
            var callbackUrl = url;
            SendEmail(user.Email, "[CASTON] Şifre Sıfırlama İsteği",
                    $"<strong>Merhaba {user.Name + " " + user.Surname}.</strong><br/>" +
                    $"Hesabınız için bir şifre sıfırlama isteği gönderildi.<br/>" +
                    $"Lütfen <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bu linke</a> tıklayarak şifrenizi sıfırlayın.<br/>" +
                    "<small>Eğer bu işlem sizin tarafınızdan gerçekleştirilmediyse lütfen bize bildirin.</small>"
                    );
        }

        public string GenerateCouponCode()
        {
            var opts = new Powells.CouponCode.Options();
            opts.PartLength = 6;
            opts.Parts = 1;
            var ccb = new CouponCodeBuilder();
            var code = ccb.Generate(opts);
            return code;
        }

        public bool RevokeSubscription(User _user)
        {
            var id = _user.Id;
            var user = context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return false;
            user.HasPaid = false;
            context.Update(user);
            context.SaveChanges();
            return true;
        }

        public static class HashString
        {
            public static string GetMD5(string text)
            {
                MD5 hash = MD5.Create();

                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }

                hash.Clear();

                return builder.ToString();
            }
        }
    }
}

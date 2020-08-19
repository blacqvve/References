using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using No2API.Data;
using No2API.Helpers;
using No2API.Entities.Models;
using No2API.Entities.Data;

namespace No2API.Controllers
{
    [Route("api/v1/feed")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    //[Authorize(Roles = "Admin")]
    public class FeedController : ControllerBase
    {
        private readonly APIContext context;
        private readonly IUtils utils;
        private readonly IHostingEnvironment hosting;

        public FeedController(APIContext _context, IUtils _utils, IHostingEnvironment _hosting)
        {
            utils = _utils;
            context = _context;
            hosting = _hosting;
        }

        [HttpGet("GetLatest")]
        [Authorize]
        public IActionResult GetLatest(int dataLength = 0)
        {
            var take = 5;
            var skip = dataLength;
            //var skip = 0;
            var content = context.Feeds
                .Include(x => x.Picture)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(x => new { x.CreatedAt, x.Id, picture = x.Picture != null ? "/content/images/" + x.Picture.Name : null, x.Text, x.User })
                .ToList();
            if (content == null)
                return NotFound();
            return Ok(content);
        }

        [HttpGet("Get/{id}")]
        [Authorize]
        public IActionResult Get(Guid id)
        {
            var content = context.Feeds
               .Include(x => x.Picture)
               .Select(x => new { x.CreatedAt, x.Id, picture = x.Picture != null ? "/content/images/" + x.Picture.Name : null, x.Text, x.User })
               .FirstOrDefault(x => x.Id == id);
            if (content == null)
                return NotFound();
            return Ok(content);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Developer, Admin")]
        public IActionResult Create([FromBody] JObject text)
        {
            try
            {
                var feed = new Feed();
                var user = context.Users.FirstOrDefault(x => x.Id == new Guid(User.Identity.Name));
                feed.CreatedAt = DateTime.Now;
                feed.User = user;
                feed.Text = text["text"].ToString();
                //if (picture != null)
                //    feed.Picture = utils.AddPicture(user, picture, "feed");
                //if (video != null)
                //    feed.Video = utils.AddVideo(user, video, "feed");
                context.Add(feed);
                context.SaveChanges();

                var oneSignalInfo = new Dictionary<string, string>()
                {
                    { "YzI2M2EyMjAtMmY0OC00NWUzLTk1NGUtNDVlZDNhYTBjYjc5", "a0bdf027-e4fc-4565-ac50-4f70077a9c36" },
                    { "ZTBiMDQ0MGItN2RlYy00N2FhLWIxMTctMWFlNmI0OGE0NGIz", "7ee8b9e9-12e0-47e2-bb8d-18a49da06069" }
                };

                foreach (var app in oneSignalInfo)
                {
                    var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
                    request.KeepAlive = true;
                    request.Method = "POST";
                    request.ContentType = "application/json; charset=utf-8";

                    request.Headers.Add("authorization", "Basic "+app.Key);
                    var obj = new
                    {
                        app_id = app.Value,
                        contents = new { en = "Yeni bir haber eklendi! İncelemek için hemen dokun." },
                        headings = new { en = "[Caston] Yeni Haber" },
                        included_segments = new string[] { "All" }
                    };
                    var param = JsonConvert.SerializeObject(obj);
                    byte[] byteArray = Encoding.UTF8.GetBytes(param);

                    string responseContent = null;

                    try
                    {
                        using (var writer = request.GetRequestStream())
                        {
                            writer.Write(byteArray, 0, byteArray.Length);
                        }

                        using (var response = request.GetResponse() as HttpWebResponse)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                        System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                    }

                    System.Diagnostics.Debug.WriteLine(responseContent);
                }
                return Ok(feed.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return BadRequest(new { message = "İşlem başarısız." });
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Developer, Admin")]
        public IActionResult Update(Guid id, string text, IFormFile picture, IFormFile video)
        {
            var feed = context.Feeds.FirstOrDefault(x => x.Id == id);
            var user = context.Users.FirstOrDefault(x => x.Id == new Guid(User.Identity.Name));
            feed.User = user;
            feed.Text = text;
            if (picture != null)
                feed.Picture = utils.AddPicture(user, picture, "feed");
            if (video != null)
                feed.Video = utils.AddVideo(user, video, "feed");
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete("Delete")]
        [Authorize(Roles = "Developer, Admin")]
        public IActionResult Delete(Guid id)
        {
            context.Remove(context.Feeds.FirstOrDefault(x => x.Id == id));
            context.SaveChanges();
            return Ok();
        }
    }
}
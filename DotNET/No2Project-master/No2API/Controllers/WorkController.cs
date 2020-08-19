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
  [Route("api/v1/work")]
  [ApiController]
  [EnableCors("AllowAnyOrigin")]
  public class WorkController : ControllerBase
  {
    private readonly APIContext context;
    private readonly IUtils utils;
    private readonly IHostingEnvironment hosting;

    public WorkController(APIContext _context, IUtils _utils, IHostingEnvironment _hosting)
    {
      utils = _utils;
      context = _context;
      hosting = _hosting;
    }

    [HttpPost("Create")]
    [Authorize]
    public IActionResult CreateWork([FromBody] JObject _work)
    {
      try
      {
        var work = new Work();
        var user = context.Users.FirstOrDefault(x => x.Id == new Guid(User.Identity.Name));
        work.CreateDate = DateTime.Now;
        work.Creator = user;
        work.Title = _work["text"]["title"].ToString();
        work.Description = _work["text"]["description"].ToString();
        work.ActiveState = true;
        context.Add(work);
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

                         request.Headers.Add("authorization", "Basic " + app.Key);
                         var obj = new
                         {
                              app_id = app.Value,
                              contents = new { en = "Yeni bir iş ilanı eklendi! İncelemek için hemen dokun." },
                              headings = new { en = "[Caston] Yeni ilan" },
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
                    return Ok(work.Id);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      return BadRequest(new { message = "İşlem başarısız." });
    }

    [HttpGet]
    [Authorize]
    [Route("CheckApplication/{workId}")]
    public IActionResult CheckApplication(Guid workId)
    {
      try
      {
        var uId = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == uId);
        var work = context.Works.FirstOrDefault(x => x.Id == workId);
        var exist = context.WorkApplications.Any(x => x.Applicant.Id == user.Id && x.Work.Id == work.Id);
        if (exist)
          return Ok(new { message = "1" });
       else
          return Ok(new { message = "2" });
      }
      catch (Exception e)
      {
        return BadRequest(new { message = "İşlem Başarısız" });
      }
    }

    [Authorize]
    [HttpGet]
    [Route("ApplyWork")]
    public IActionResult ApplyWork(Guid workId)
    {
      //"08d79f61-936a-9efa-472e-d2f4239a8aa0"
      try
      {
        Guid u_Id = Guid.Parse(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        var work = context.Works.FirstOrDefault(x => x.Id == workId);
        if (work == null || work.ActiveState == false)
          return BadRequest(new { message = "Başvuru yaptığınız iş kapatılmıştır" });
        var exist = context.WorkApplications.Any(x => x.Applicant.Id == user.Id&&x.Work.Id==work.Id);
        if (exist)
          return BadRequest(new { message = "Bu iş ilanına daha önce başvuru yaptınız" });

        var workApl = new WorkApplications()
        {
          ApplyDate = DateTime.Now,
          Applicant = user,
          Work = work
        };
        context.Add(workApl);
        context.SaveChanges();
        return Ok(workApl.Id);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      return BadRequest(new { message = "İşlem başarısız." });
    }

    [HttpPost("RemoveApplication")]
    [Authorize]
    public IActionResult RemoveApplication(Guid workId)
    {
      try
      {
        var u_Id = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        if (user == null)
          return Unauthorized();
        var work = context.Works.FirstOrDefault(x => x.Id == workId);
        var workapplication = context.WorkApplications.FirstOrDefault(x => x.Work == work && x.Applicant == user);
        context.Remove(workapplication);
        context.SaveChanges();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(new { message = "İşlem Başarısız" });

      }
    }

    [HttpGet("Deactivate")]
    [Authorize]
    public IActionResult DeactivateWork([FromQuery]Guid workId)
    {
      try
      {
        var u_Id = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        var work = context.Works.FirstOrDefault(x => x.Id == workId);
        if (work == null)
          return BadRequest(new { message = "Kapatmaya çalıştığınız iş ilanı bulunamadı" });
        if (user != work.Creator)
          return BadRequest(new { message = "Bu iş ilanı size ait değil" });
        if (work.ActiveState == false)
          return BadRequest(new { message = "Bu iş ilanı kapalı durumda" });
        work.ClosingDate = DateTime.Now;
        work.ActiveState = false;
        context.Update(work);
        context.SaveChanges();
        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return BadRequest(new { message = "İşlem başarısız." });
    }

    [HttpGet]
    [Route("ActivateWork/{workId}")]
    [Authorize]

    public IActionResult ActivateWork(Guid workId)
    {
      try
      {
        var u_Id = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        var work = context.Works.FirstOrDefault(x => x.Id == workId);
        if (work == null)
          return BadRequest(new { message = "Aktive etmeye çalıştığınız iş bulunamadı" });
        if (user != work.Creator)
          return BadRequest(new { message = "Bu iş ilanı size ait değil" });
        if (work.ActiveState == true)
          return BadRequest(new { message = "Bu iş ilanı açık durumda" });

        work.ClosingDate = null;
        work.ActiveState = true;
        context.Update(work);
        context.SaveChanges();
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "İşlem Başarısız" });
      }
    }

    [HttpGet]
    [Route("GetLatest")]
    [Authorize]

    public IActionResult GetLatest(int dataLength = 0)
    {
      try
      {
        var take = 5;
        var skip = dataLength;
        var content = context.Works
          .OrderByDescending(x => x.CreateDate)
          .Skip(skip)
          .Take(take)
          .Where(x => x.ActiveState == true)
          .ToList();
        if (content == null)
          return BadRequest(new { message = "kayıt bulunamadı" });
        return Ok(content);
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = "İşlem başarısız" });
      }
    
    }

    [HttpGet("GetWork/{id}")]
    [Authorize]
    public IActionResult GetWork(Guid id)
    {
      try
      {
        var content = context.Works
            .FirstOrDefault(x => x.Id == id);
        if (content == null)
          return NotFound();
        
        return Ok(content);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return BadRequest(new { message = "İşlem başarısız" });
      }
    }

    [HttpPost("Update/{id}")]
    [Authorize]
    public IActionResult UpdateWork([FromBody]  JObject _work,Guid id)
    {
      try
      {
        var work = new Work();
        work = context.Works.FirstOrDefault(x => x.Id ==id);
        if (work == null)
          return BadRequest(new { message = "Düzenleme yapmaya çalıştığınız iş bulunamadı" });

        work.Title = _work["text"]["title"].ToString();
        work.Description = _work["text"]["description"].ToString();

        context.Update(work);
        context.SaveChanges();
        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return BadRequest(new { message = "İşlem Başarısız" });

      }
    
    }
    [HttpDelete("Delete/{id}")]
    [Authorize]

    public IActionResult Delete(Guid id)
    {
      try
      {
        var u_Id = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        var work = context.Works.FirstOrDefault(x => x.Id == id);
        if (work == null)
          return BadRequest(new { message = "Silmeye çalıştığınız iş bulunamadı" });
        var works = context.WorkApplications.Where(x => x.Work == work).ToList();
        context.RemoveRange(works);
        context.Remove(work);
        context.SaveChanges();
        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
      return BadRequest(new { message = "İşlem Başarısız" });
    }
   
    [HttpGet]
    [Route("GetCreatorWorks")]
    [Authorize]

    public IActionResult GetCreatorWorks()
    {
      try
      {
        var u_Id = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        var content = context.Works
          .OrderByDescending(x => x.CreateDate)
          .Where(x => x.Creator == user)
          .ToList();
        if (content == null)
          return BadRequest(new { message = "Kayıt bulunamadı" });
        return Ok(content);

      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet]
    [Route("GetApplicants/{id}/{dataLength}")]
    [Authorize]

    public IActionResult GetApplicants(Guid id,int dataLength)
    {
      try
      {
        var u_Id = new Guid(User.Identity.Name);
        var user = context.Users.FirstOrDefault(x => x.Id == u_Id);
        var work = context.Works.FirstOrDefault(x => x.Id == id);
        if (work.Creator != user)
          return BadRequest(new { message = "İş ilanı size ait olmadığı için başvuran üyeleri göremezsiniz" });
        if (work == null)
          return BadRequest(new { message = "İş ilanı bulunamadı" });
        var content = context.WorkApplications
          .OrderByDescending(x => x.ApplyDate)
          .Skip(dataLength)
          .Take(20)
          .Where(x => x.Work.Id == work.Id)
          .Select(x=>x.Applicant)
          .ToList();
        return Ok(content);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return BadRequest(new { message = "İşlem Başarısız" });
      }
    }




  }





}
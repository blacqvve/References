using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using No2Web.Data;
using No2API.Entities.Models;
using static No2Web.Helpers.Utils;

namespace No2Web.Controllers
{
  public class ProcessController : Controller
  {
    private readonly DataContext context;

    public ProcessController(DataContext context)
    {
      this.context = context;
    }

    public IActionResult ConfirmEmail(string token)
    {
      if (token == null)
      {
        ViewBag.Error = "Hatalı kod girişi.";
        return View();
      }
      var user = context.Users.Where(x => x.MailConfirmationToken == token).FirstOrDefault();
      if (user == null)
      {
        ViewBag.Error = "Hatalı kod girişi.";
        return View();
      }
      if (user.ConfirmedEmail == true)
      {
        ViewBag.Error = "Bu email adresi zaten doğrulanmış.";
        return View();
      }
      user.ConfirmedEmail = true;
      context.Users.Update(user);
      context.SaveChanges();
      return View();
    }

    public IActionResult ResetPassword(string token)
    {
      if (token == null)
      {
        ViewBag.Error = "Yanlış veya süresi geçmiş bir kod.";
        return View();
      }
      ViewBag.Token = token;
      var user = context.Users.FirstOrDefault(x => x.ResetPasswordToken == token);
      if (user == null)
      {
        ViewBag.Error = "Yanlış veya süresi geçmiş bir kod.";
        return View();
      }
      return View(user); //view here
    }

    [HttpPost]
    public IActionResult ResetPassword(User info, string token)
    {
      var user = context.Users.FirstOrDefault(x => x.Id == info.Id);
      if (user == null)
      {
        ViewBag.Error = "Yanlış veya süresi geçmiş bir kod.";
        return View();
      }
      if (user.ResetPasswordToken != info.ResetPasswordToken)
      {
        ViewBag.Error = "Yanlış veya süresi geçmiş bir kod.";
        return View();
      }
      user.Password = HashString.GetMD5(info.Password);
      context.Update(user);
      context.SaveChanges();
      ViewBag.Success = "Şifre başarıyla güncellendi.";
      return View();
    }
  }
}
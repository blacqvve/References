using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MagicBox.WebAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MagicBox.Data.Models;

namespace MagicBox.WebAdmin.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly UserManager<User> userManager;

    public HomeController(UserManager<User> userManager)
    {
      this.userManager = userManager;
    }

    public IActionResult Index()
    {
      if (User.IsInRole("Admin"))
        return RedirectToAction("Index", "Company");
      return RedirectToAction("Manage", "CouponCode", new { id = userManager.GetUserId(User) });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}

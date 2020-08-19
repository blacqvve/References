using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using No2API.Entities.Models;
using No2Manager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace No2Manager.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
      _context = context;
    }
    public IActionResult Index()
    {
      return View();
    }
    public async Task<IActionResult> List()
    {
      var model = _context.Users.Include(x => x.Referrer).ToList();
      return View(model);
    }
    public async Task<IActionResult> SalesmanList()
    {
      var model = await _context.Users.Include(x => x.Referrer).Where(x => x.Role == "Salesman").ToListAsync();
      return View(model);
    }
    public async Task<IActionResult> MonthlySubs()
    {
      var model = await _context.Users.Include(x => x.Referrer).Where(x => x.ActivationDate.Month == DateTime.Now.Month).ToListAsync();
      var typeOne = await _context.Users.Include(x => x.Referrer).GroupBy(x => x.Subscription == "tip1").CountAsync();
      var typeTwo = await _context.Users.Include(x => x.Referrer).GroupBy(x => x.Subscription == "tip2").CountAsync();
      var typeThree = await _context.Users.Include(x => x.Referrer).GroupBy(x => x.Subscription == "tip3").CountAsync();
      return View(model);
    }
    public async Task<IActionResult> SalesmanSale(Guid salerId)
    {
      var saler = await _context.Users.FirstOrDefaultAsync(x => x.Id == salerId);
      var model = await _context.Users.Include(x => x.Referrer).Where(x => x.Referrer == saler).ToListAsync();
      return View(model);
    }

    public IActionResult Privacy()
    {
      return View();
    }

    public IActionResult ActivateAccount()
    {
      return View();
    }

    [HttpPost]
    public IActionResult ActivateAccount(IFormCollection form)
    {
      if (string.IsNullOrEmpty(form["UserId"]) || string.IsNullOrEmpty(form["Subscription"]))
      {
        ViewBag.Error = "Lütfen tüm alanları doldurun.";
        return View();
      }
      var user = _context.Users.Include(x=>x.Orders).FirstOrDefault(x => x.Id == new Guid (form["UserId"]));
      if (user == null)
      {
        ViewBag.Error = "Kullanıcı bulunamadı.";
        return View();
      }
      user.HasPaid = true;
      user.ActivationDate = DateTime.Now;
      user.ExpirationDate = DateTime.Now.AddMonths(Int32.Parse(form["Subscription"]));
      if (user.Orders.Any())
        _context.Orders.RemoveRange(user.Orders);
      _context.Orders.Add(new Order()
      {
        User = user,
        OrderId = "MANUALORDER_" + user.Id
      });

      _context.Update(user);
      _context.SaveChanges();

      ViewBag.Success = "Üyelik başarıyla aktifleştirildi.";

      return View();
    }
  }
}

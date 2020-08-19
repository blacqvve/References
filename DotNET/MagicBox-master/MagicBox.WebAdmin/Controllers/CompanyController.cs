using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MagicBox.Data.Data;
using MagicBox.Data.Helpers;
using MagicBox.Data.Managers;
using MagicBox.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MagicBox.WebAdmin.Controllers
{
  [Authorize(Roles = "Admin")]
  public class CompanyController : Controller
  {
    private readonly UserManager<User> userManager;
    private readonly IPlayerManager playerManager;
    private readonly ICouponManager couponManager;
    private readonly DataContext context;
    private readonly IHostingEnvironment hosting;

    public CompanyController(UserManager<User> userManager,
                             IPlayerManager playerManager,
                             ICouponManager couponManager,
                             DataContext context,
                             IHostingEnvironment hosting)
    {
      this.userManager = userManager;
      this.playerManager = playerManager;
      this.couponManager = couponManager;
      this.context = context;
      this.hosting = hosting;
    }

    public async Task<IActionResult> Index()
    {
      var companies = await userManager.GetUsersInRoleAsync("Company");
      var userinfos = playerManager.GetUserInfos(companies.ToList());
      return View(userinfos.ToList());
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserInfo userInfo)
    {
      var result = await userManager.CreateAsync(new Data.Models.User
      {
        Email = userInfo.Email,
        UserName = userInfo.Email
      }, userInfo.Password);

      if (result == IdentityResult.Success)
      {
        var user = await userManager.FindByEmailAsync(userInfo.Email);
        var roleAddedResult = await userManager.AddToRoleAsync(user, "Company");
        if (roleAddedResult.Succeeded)
        {
          userInfo.CreatedAt = DateTime.Now;
          userInfo.User = user;
          userInfo.UserId = user.Id;

          if (userInfo.File != null)
          {
            var filePath = Path.Combine(hosting.WebRootPath, "uploads/files");
            if (!Directory.Exists(filePath))
            {
              Directory.CreateDirectory(filePath);
            }
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + Path.GetExtension(userInfo.File.FileName);
            using (var fileStream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
            {
              await userInfo.File.CopyToAsync(fileStream);
            }
            userInfo.Picture = Utils.GetImageAsByte(Path.Combine(filePath, fileName));
            System.IO.File.Delete(Path.Combine(filePath, fileName));
          }

          playerManager.AddUserInfo(userInfo);
          return RedirectToAction(nameof(Index));
        }
      }
      return View(userInfo);
    }

    public async Task<IActionResult> Delete(string id)
    {
      return View(await userManager.FindByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      var userInfo = playerManager.GetPlayerInfo(user);
      var coupons = couponManager.GetCouponsBycreator(user);
      var shopItems = playerManager.GetPlayerItems(user);
      context.RemoveRange(coupons);
      context.RemoveRange(shopItems);
      context.RemoveRange(userInfo);
      await context.SaveChangesAsync();
      await userManager.DeleteAsync(user);
      return RedirectToAction(nameof(Index));
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicBox.Data.Helpers;
using MagicBox.Data.Managers;
using MagicBox.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MagicBox.WebAdmin.Controllers
{
  public class CouponCodeController : Controller
  {
    private readonly UserManager<User> userManager;
    private readonly ICouponManager couponManager;
    private readonly IPlayerManager playerManager;

    public CouponCodeController(UserManager<User> userManager,
                                ICouponManager couponManager,
                                IPlayerManager playerManager)
    {
      this.userManager = userManager;
      this.couponManager = couponManager;
      this.playerManager = playerManager;
    }

    public IActionResult Index()
    {
      //return View(couponManager.GetCouponCodes(false));
      return NotFound();
    }

    public async Task<IActionResult> Manage(string id)
    {
      var user = await userManager.FindByIdAsync(id);
      if (user == null)
        return NotFound();
      var coupons = couponManager.GetCouponsBycreator(user);
      if (coupons == null)
        return NotFound();
      return View(coupons);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(string id)
    {
      if (string.IsNullOrEmpty(id))
        return BadRequest();
      var user = await userManager.FindByIdAsync(id);
      if (user == null)
        return NotFound();
      var model = new CouponCode();
      model.Creator = user;
      return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CouponCode coupon)
    {
      var user = await userManager.FindByIdAsync(coupon.Creator.Id);
      if (user == null)
        return NotFound();
      var code = couponManager.AddCouponCode(coupon);
      return RedirectToAction("Manage", new { id = code.Creator.Id });
    }

    [Authorize]
    public IActionResult ToggleActivate(Guid id, string CompanyId)
    {
      var code = couponManager.GetCouponCodeById(id);
      couponManager.ActivateCouponCode(code, !code.Active);
      return RedirectToAction(nameof(Manage), "CouponCode", new { id = CompanyId });
    }
  }
}
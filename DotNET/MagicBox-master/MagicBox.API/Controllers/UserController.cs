using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagicBox.Data.Helpers;
using MagicBox.Data.Managers;
using MagicBox.Data.Data;
using MagicBox.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static MagicBox.Data.Helpers.Enums;

namespace MagicBox.API.Controllers
{
  [Route("api/[controller]/v1")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly DataContext context;
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly SignInManager<User> signInManager;
    private readonly ICouponManager couponManager;
    private readonly IPlayerManager playerManager;

    public UserController(
      DataContext context,
      UserManager<User> userManager,
      RoleManager<IdentityRole> roleManager,
      SignInManager<User> signInManager,
      ICouponManager couponManager,
      IPlayerManager playerManager)
    {
      this.context = context;
      this.userManager = userManager;
      this.roleManager = roleManager;
      this.signInManager = signInManager;
      this.couponManager = couponManager;
      this.playerManager = playerManager;
    }

    // GET api/user/v1
    [HttpGet]
    public IActionResult Get()
    {
      return Ok("Discountbox API is operational.");
    }

    [HttpGet("IsLoggedIn")]
    public IActionResult IsLoggedIn()
    {
      return Ok(signInManager.IsSignedIn(User));
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] UserInfo player)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(modelError => modelError.ErrorMessage).ToList());
      }

      if (string.IsNullOrEmpty(player.Email) || string.IsNullOrEmpty(player.Password))
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.EMPTY_MAIL));

      var user = new User
      {
        UserName = player.Email,
        Email = player.Email,
      };
      user.UserInfo = new UserInfo
      {
        CreatedAt = DateTime.Now,
        Gender = player.Gender,
        Points = player.Points,
        City = player.City,
      };
      var result = await userManager.CreateAsync(user, player.Password);

      if (!result.Succeeded)
      {
        return BadRequest(result.Errors.Select(x => x.Description).ToList());
      }

      await signInManager.SignInAsync(user, true);
      await userManager.AddToRoleAsync(user, "Player");

      context.SaveChanges();

      return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] UserInfo _player)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(modelError => modelError.ErrorMessage).ToString());
      }

      var player = await userManager.FindByEmailAsync(_player.Email);

      if (string.IsNullOrEmpty(_player.Email) || string.IsNullOrEmpty(_player.Password))
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.EMPTY_MAIL));

      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.WRONG_MAIL_OR_PW));


      var signedIn = await signInManager.CheckPasswordSignInAsync(player, _player.Password, true);
      if (signedIn == Microsoft.AspNetCore.Identity.SignInResult.Success)
      {
        await signInManager.SignInAsync(player, true);
        return Ok();
      }
      else if (signedIn == Microsoft.AspNetCore.Identity.SignInResult.LockedOut)
      {
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.LOCKOUT));
      }
      else if (signedIn == Microsoft.AspNetCore.Identity.SignInResult.Failed)
      {
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.WRONG_MAIL_OR_PW));
      }
      else
      {
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.UNABLE_TO_LOGIN));
      }
    }

    [Authorize]
    [HttpPost("UpdatePlayerLocation")]
    public async Task<IActionResult> UpdatePlayerLocation([FromForm] string city)
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      if (string.IsNullOrEmpty(city))
        return BadRequest(string.Format(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.EMPTY_FIELD), "City"));
      var result = await userManager.UpdateAsync(player);
      if (result == IdentityResult.Success)
      {
        playerManager.UpdatePlayerLocation(player, city);
        return Ok();
      }
      return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.UNKNOWN_ERROR));
    }

    [Authorize]
    [HttpPost("AddPoints")]
    public async Task<IActionResult> AddPoints([FromForm] float? points)
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      if (points == null)
        return BadRequest(string.Format(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.EMPTY_FIELD), "Points"));
      var result = await userManager.UpdateAsync(player);
      if (result == IdentityResult.Success)
      {
        playerManager.AddPlayerPoints(player, (float)points);
        return Ok();
      }
      return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.UNKNOWN_ERROR));
    }

    [Authorize]
    [HttpGet("GetPoints")]
    public async Task<IActionResult> GetPoints()
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      return Ok(playerManager.GetPlayerInfo(player).Points);
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      await signInManager.SignOutAsync();

      return Ok();
    }

    [Authorize]
    [HttpGet("GetShopItems")]
    public async Task<IActionResult> GetShopItems([FromQuery] bool PlayerSpecific)
    {
      if (PlayerSpecific)
      {
        var player = await userManager.FindByNameAsync(User.Identity.Name);
        if (player == null)
          return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
        return Ok(playerManager.GetPlayerItems(player));
      }

      var data = context.ShopItems
        .Include(x => x.CouponCode)
          .ThenInclude(x=>x.Picture)
        .Include(x=>x.CouponCode)
          .ThenInclude(x=>x.Creator)
        .ToList();

      return Ok(data);
    }

    [Authorize]
    [HttpPost("BuyShopItem")]
    public async Task<IActionResult> BuyShopItem([FromForm] ShopItem _item)
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      var item = await context.ShopItems.Include(x=>x.CouponCode).FirstOrDefaultAsync(x => x.Id == _item.Id || x.Name == _item.Name);
      if (item == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(TransactionStatus.UNABLE_TO_FIND_ITEM));
      var result = couponManager.BuyShopItem(player, item); //doldur
      if (result == TransactionStatus.INSUFFICIENT_FUNDS)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(TransactionStatus.INSUFFICIENT_FUNDS));
      return Ok();
    }

    [Authorize]
    [HttpGet("GetPlayerCoupons")]
    public async Task<IActionResult> GetPlayerCoupons()
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      return Ok(context.CouponCodes.Where(code => code.User.Id == player.Id).Select(code => new { code }).ToList());
    }

    [Authorize]
    [HttpPost("AddPlayerCoupon")]
    public async Task<IActionResult> AddPlayerCoupon([FromForm] Guid CouponId)
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      var coupon = couponManager.AssignCouponToPlayer(player, couponManager.GetCouponCodeById(CouponId));
      if (coupon == null)
      {
        coupon = couponManager.AddCouponCode();
        couponManager.AssignCouponToPlayer(player, coupon);
      }
      return Ok(coupon.Id);
    }

    [Authorize]
    [HttpGet("GetPlayerGender")]
    public async Task<IActionResult> GetPlayerGender()
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      var info = playerManager.GetPlayerInfo(player);
      if (info == null)
        return NotFound();
      return Ok(info.Gender);
    }

    [Authorize]
    [HttpGet("GetPlayerInfo")]
    public async Task<IActionResult> GetPlayerInfo()
    {
      var player = await userManager.FindByNameAsync(User.Identity.Name);
      if (player == null)
        return BadRequest(Utils.UserAPI.GetErrorStringFormatted(UserAPIBadRequest.SESSION_DROPPED));
      var info = playerManager.GetPlayerInfo(player);
      if (info == null)
        return NotFound();
      return Ok(info);
    }
  }
}


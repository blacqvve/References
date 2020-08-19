using MagicBox.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Powells.CouponCode;
using MagicBox.Data.Models;
using MagicBox.Data.Helpers;
using static MagicBox.Data.Helpers.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MagicBox.Data.Managers
{
  public interface ICouponManager
  {
    CouponCode AddCouponCode();
    CouponCode AddCouponCode(CouponCode _code, int digits = 6);
    CouponCode AssignCouponToPlayer(User player, CouponCode coupon);
    CouponCode GetCouponCodeById(Guid Id);
    List<CouponCode> GetCouponCodes(bool onlyActiveCodes);
    TransactionStatus BuyShopItem(User player, ShopItem item);
    bool IsClaimed(CouponCode code);
    bool IsClaimed(ShopItem item);
    IEnumerable<CouponCode> GetCouponsBycreator(User creator);
    void ActivateCouponCode(CouponCode _c, bool active);
  }

  public class CouponManager : ICouponManager
  {
    private readonly DataContext context;
    private readonly UserManager<User> userManager;
    private readonly IPlayerManager playerManager;

    public CouponManager(
      DataContext context,
      UserManager<User> userManager,
      IPlayerManager playerManager
      )
    {
      this.context = context;
      this.userManager = userManager;
      this.playerManager = playerManager;
    }

    public List<CouponCode> GetCouponCodes(bool onlyActiveCodes = true)
    {
      return onlyActiveCodes ? context.CouponCodes
                               .Where(x => x.Active)
                               .Include(x => x.Creator)
                               .ToList() : context.CouponCodes.Include(x => x.Creator).ToList();
    }

    public IEnumerable<CouponCode> GetCouponsBycreator(User creator)
    {
      if (creator != null)
        return GetCouponCodes(false).Where(x => x.Creator.Id == creator.Id).ToList();
      return null;
    }

    public CouponCode AddCouponCode()
    {
      CouponCode coupon = new CouponCode
      {
        Active = true,
        CreatedAt = DateTime.Now,
        Category = "",
        Code = Utils.UserAPI.GenerateCouponCode(6),

      };
      context.Add(coupon);
      context.SaveChanges();
      return coupon;
    }

    public CouponCode AddCouponCode(CouponCode _code, int digits = 6)
    {
      _code.Id = Guid.NewGuid();
      _code.CreatedAt = DateTime.Now;
      _code.Code = string.IsNullOrEmpty(_code.Code) ? Utils.UserAPI.GenerateCouponCode(digits) : _code.Code;
      context.Add(_code);
      context.SaveChanges();
      return _code;
    }

    public void ActivateCouponCode(CouponCode _c, bool active)
    {
      var code = context.CouponCodes.FirstOrDefault(x => x.Id == _c.Id);
      code.Active = active;
      context.Update(code);
      context.SaveChanges();
    }

    public CouponCode AssignCouponToPlayer(User player, CouponCode coupon)
    {

      if (coupon != null)
      {
        var _c = context.CouponCodes.FirstOrDefault(x => x.Id == coupon.Id);
        if (_c != null)
        {
          _c.User = player;
          context.Update(_c);
          context.SaveChanges();
          return _c;
        }
      }
      return null;
    }

    public bool IsClaimed(CouponCode code)
    {
      var _c = context.CouponCodes.Include(x => x.User).FirstOrDefault(x => x.Id == code.Id);
      if (_c != null)
        return _c.User != null;
      return false;
    }

    public bool IsClaimed(ShopItem item)
    {
      var _c = context.ShopItems
        .Include(x => x.CouponCode)
        .Include(x => x.CouponCode)
          .ThenInclude(x => x.User)
        .FirstOrDefault(x => x.Id == item.Id);
      if (_c != null)
        return _c.CouponCode.User != null;
      return false;
    }

    public CouponCode GetCouponCodeById(Guid Id)
    {
      return context.CouponCodes.FirstOrDefault(x => x.Id == Id);
    }

    public bool IsShopItem(CouponCode code)
    {
      return context.ShopItems.Any(x => x.CouponCode.Id == code.Id);
    }

    public void AddCouponCodePicture(CouponCode code, byte[] picData)
    {
      var pic = new Picture()
      {
        CouponCode = code,
        Data = picData,
        CreatedAt = DateTime.Now
      };

      context.Add(pic);
      context.SaveChanges();
    }

    public TransactionStatus BuyShopItem(User player, ShopItem item)
    {
      var points = playerManager.GetPlayerInfo(player).Points;
      if (points - item.Price < 0)
        return TransactionStatus.INSUFFICIENT_FUNDS;

      playerManager.RemovePlayerPoints(player, item.Price);

      var coupon = AssignCouponToPlayer(player, item.CouponCode);
      if (coupon == null)
        return TransactionStatus.UNKNOWN_ERROR;

      return TransactionStatus.OK;
    }
  }
}

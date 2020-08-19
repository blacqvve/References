using MagicBox.Data.Data;
using MagicBox.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Managers
{
  public interface IPlayerManager
  {
    UserInfo GetPlayerInfo(User player);
    bool AddPlayerPoints(User player, float points);
    bool RemovePlayerPoints(User player, float points);
    bool UpdatePlayerLocation(User player, string city);
    List<ShopItem> GetPlayerItems(User player);
    IEnumerable<UserInfo> GetUserInfos(List<User> users);
    void AddUserInfo(UserInfo userInfo);
  }

  public class PlayerManager : IPlayerManager
  {
    private readonly DataContext context;
    public PlayerManager(DataContext context)
    {
      this.context = context;
    }

    public UserInfo GetPlayerInfo(User player)
    {
      return context.UserInfos.FirstOrDefault(x => x.UserId == player.Id);
    }

    public IEnumerable<UserInfo> GetUserInfos(List<User> users)
    {
      var userinfos = new List<UserInfo>();
      users.ForEach(
        user => {
          userinfos.Add(context.UserInfos
          .Where(x => x.User.Id == user.Id)
          .FirstOrDefault());
        });
      
      return userinfos.Where(x => x != null).ToList();
    }

    public void AddUserInfo(UserInfo userInfo)
    {
      context.UserInfos.Add(userInfo);
      context.SaveChanges();
    }

    public List<ShopItem> GetPlayerItems(User player)
    {
      return context.ShopItems
        .Include(x => x.CouponCode)
          .ThenInclude(x => x.Picture)
        .Include(x => x.CouponCode)
            .ThenInclude(x => x.Creator)
        .Select(x => x)
        .Where(x => x.CouponCode.User.Id == player.Id)
        .ToList();
    }

    public bool AddPlayerPoints(User player, float points)
    {
      try
      {
        var info = GetPlayerInfo(player);
        if (info == null)
          return false;
        info.Points += points;
        context.Update(info);
        context.SaveChanges();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public bool RemovePlayerPoints(User player, float points)
    {
      try
      {
        var info = GetPlayerInfo(player);
        if (info == null)
          return false;
        info.Points -= points;
        context.Update(info);
        context.SaveChanges();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public bool UpdatePlayerLocation(User player, string city)
    {
      try
      {
        var info = GetPlayerInfo(player);
        if (info == null)
          return false;
        info.City += city;
        context.Update(info);
        context.SaveChanges();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
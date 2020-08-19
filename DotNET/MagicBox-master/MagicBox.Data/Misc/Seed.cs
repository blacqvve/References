using MagicBox.Data.Helpers;
using MagicBox.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Data
{
  public static class Seed
  {
    public static async Task AddCouponCodes(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, int iteration = 15)
    {
      context.Database.Migrate();

      foreach (var role in new string[] { "Admin", "Company", "Player" })
      {
        var exists = await roleManager.FindByNameAsync(role);
        if (exists == null)
        {
          await roleManager.CreateAsync(new IdentityRole { Name = role });
        }
      }

      var user = await userManager.FindByEmailAsync("admin@admin");

      if (user == null)
      {
        user = new User
        {
          UserName = "admin@admin",
          Email = "admin@admin",
        };
        user.UserInfo = new UserInfo
        {
          CreatedAt = DateTime.Now,
          Gender = 1,
          Points = 0,
          City = "Ankara",
        };
        await userManager.CreateAsync(user, "Test123");
      }

      var inRole = await userManager.IsInRoleAsync(user, "Admin");
      if (!inRole)
        await userManager.AddToRoleAsync(user, "Admin");

      if (context.CouponCodes.Any())
        return;
      for (int i = 0; i < iteration; i++)
      {

        var couponcode = new CouponCode
        {
          Active = true,
          CreatedAt = DateTime.Now,
          Category = "",
          Code = Utils.UserAPI.GenerateCouponCode(),
          Creator = user,
          User = user
        };
        var r = new Random();
        var shopItem = new ShopItem() { CouponCode = couponcode, CreatedAt = DateTime.Now, Description = "", Name = couponcode.Code, Price = r.Next(), Stock = r.Next() };
        context.Add(shopItem);
        context.Add(couponcode);
        context.SaveChanges();
      }
    }
  }
}

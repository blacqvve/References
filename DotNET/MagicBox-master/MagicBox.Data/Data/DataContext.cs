using MagicBox.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagicBox.Data.Data
{
  public class DataContext : IdentityDbContext<User>
  {
    public static string ConnectionString { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseMySql(ConnectionString);
    }
    public DbSet<CouponCode> CouponCodes { get; set; }
    public DbSet<Picture> Pictures { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<ShopItem> ShopItems { get; set; }
    public DbSet<UserInfo> UserInfos { get; set; }
    public DbSet<UserItem> UserItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<UserItem>()
          .HasKey(t => new { t.ShopItemId, t.PlayerId });

      modelBuilder.Entity<UserItem>()
          .HasOne(pt => pt.Player)
          .WithMany(p => p.UserItems)
          .HasForeignKey(pt => pt.PlayerId);

      modelBuilder.Entity<UserItem>()
          .HasOne(pt => pt.ShopItem)
          .WithMany(t => t.UserItems)
          .HasForeignKey(pt => pt.ShopItemId);

      modelBuilder.Entity<User>()
        .HasOne(x => x.UserInfo)
        .WithOne(x => x.User)
        .HasForeignKey<UserInfo>(x => x.UserId);

      modelBuilder.Entity<User>()
        .HasOne(x => x.UserInfo)
        .WithOne(x => x.User)
        .HasForeignKey<UserInfo>(x => x.UserId);

      modelBuilder.Entity<CouponCode>()
        .HasOne(x => x.Picture)
        .WithOne(x => x.CouponCode)
        .HasForeignKey<Picture>(x => x.CouponCodeId);

      base.OnModelCreating(modelBuilder);
    }
  }
}
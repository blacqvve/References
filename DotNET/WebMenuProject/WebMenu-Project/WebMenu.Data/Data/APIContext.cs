using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebMenu.Data.Models;

namespace WebMenu.Data.Data
{
     public class APIContext : IdentityDbContext<User>
     {
          public static string ConnectionString { get; set; }

          protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          {
               optionsBuilder.UseMySql(ConnectionString);

          }
          public DbSet<UserInfo> UserInfos { get; set; }

          public DbSet<Company> Companies { get; set; }

          public DbSet<Picture> Pictures { get; set; }

          public DbSet<Menu> Menus { get; set; }

          public DbSet<Category> Categories { get; set; }

          public DbSet<MenuItem> MenuItems { get; set; }
          protected override void OnModelCreating(ModelBuilder modelBuilder)
          {
               base.OnModelCreating(modelBuilder);

               modelBuilder.Entity<IdentityUser>(entity => entity.Property(m => m.Id).HasMaxLength(127));
               modelBuilder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(127));
               modelBuilder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(127));

               modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(127));
               modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(127));

               modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(127));
               modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(127));
               modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
               modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));

               modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(127));

               modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
               modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(127));
               modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(127));

               modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(127));
               modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
               modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(127));
               modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(127));

               modelBuilder.Entity<Company>().HasOne(x => x.Menu).WithOne(x => x.Company).HasForeignKey<Menu>(x=>x.CompanyId).OnDelete(DeleteBehavior.Cascade);

               modelBuilder.Entity<MenuItem>().HasMany(x => x.Pictures).WithOne().OnDelete(DeleteBehavior.Cascade);

               modelBuilder.Entity<Menu>().HasMany(x => x.Categories).WithOne().OnDelete(DeleteBehavior.Cascade).IsRequired();

               modelBuilder.Entity<Category>().HasMany(x => x.MenuItems).WithOne(x => x.Category).OnDelete(DeleteBehavior.Cascade);

               

               modelBuilder.Entity<User>()
  .HasOne(x => x.UserInfo)
  .WithOne(x => x.User)
  .HasForeignKey<UserInfo>(x => x.UserId)
  .OnDelete(DeleteBehavior.Cascade);

          }
     }

}

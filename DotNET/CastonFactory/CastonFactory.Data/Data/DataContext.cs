using CastonFactory.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CastonFactory.Data.Data
{
     public class DataContext : IdentityDbContext<User>
     {
          public DataContext(DbContextOptions<DataContext> options)
                      : base(options)
          {
          }
          public DbSet<Content> Contents { get; set; }

          public DbSet<Theme> Themes { get; set; }

          public DbSet<Genre> Genres { get; set; }

          public DbSet<ContentData> ContentDatas { get; set; }

          public DbSet<SupportRequest> SupportRequests { get; set; }

          public DbSet<ContentRating> ContentRatings { get; set; }

          protected override void OnModelCreating(ModelBuilder builder)
          {
               base.OnModelCreating(builder);
               builder.Entity<User>(entity => entity.Property(m => m.Id).HasMaxLength(85));
               builder.Entity<User>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
               builder.Entity<User>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

               builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
               builder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

               builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
               builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
               builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
               builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

               builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

               builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
               builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
               builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

               builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
               builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
               builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
               builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

               builder.Entity<Theme>().HasMany(x => x.Contents).WithOne(x => x.Theme).OnDelete(DeleteBehavior.Restrict);
               builder.Entity<Genre>().HasMany(x => x.Contents).WithOne(x => x.Genre).OnDelete(DeleteBehavior.Restrict);
               builder.Entity<Content>().HasOne(x => x.Theme).WithMany(x => x.Contents).OnDelete(DeleteBehavior.Restrict);
               builder.Entity<Content>().HasOne(x => x.Genre).WithMany(x => x.Contents).OnDelete(DeleteBehavior.Restrict);

               builder.Entity<User>().HasMany(x => x.Contents).WithOne(x => x.User).OnDelete(DeleteBehavior.Restrict);
               builder.Entity<Content>().HasOne(x => x.User).WithMany(x => x.Contents).OnDelete(DeleteBehavior.Restrict);

               // Use the shadow property as a foreign key
               builder.Entity<Content>()
                   .HasOne(p => p.Data).WithOne().HasForeignKey<ContentData>(x => x.ContentId).OnDelete(DeleteBehavior.Cascade);

               builder.Entity<SupportRequest>().HasOne(x => x.User).WithMany(x=>x.SupportRequests).OnDelete(DeleteBehavior.Cascade);
               builder.Entity<User>().HasMany(x => x.SupportRequests).WithOne(x => x.User).OnDelete(DeleteBehavior.Restrict);
               
               builder.Entity<Content>().HasOne(x => x.Rating).WithOne().HasForeignKey<ContentRating>(x=>x.ContentID).IsRequired(false).OnDelete(DeleteBehavior.Cascade);

               //var uId = Guid.NewGuid().ToString();
               //var rId = Guid.NewGuid().ToString();
               //var cpId = Guid.NewGuid().ToString();
               // any guid, but nothing is against to use the same one

               //builder.Entity<IdentityRole>().HasData(new IdentityRole
               //{
               //    Id = rId,
               //    Name = "Admin",
               //    NormalizedName = "admin"
               //});
               //builder.Entity<IdentityRole>().HasData(new IdentityRole
               //{
               //    Id = Guid.NewGuid().ToString(),
               //    Name = "Otoriter",
               //    NormalizedName = "otoriter"
               //});
               //builder.Entity<IdentityRole>().HasData(new IdentityRole
               //{
               //     Id = cpId,
               //     Name = "ContentProducer",
               //     NormalizedName = "contentproducer"
               //});

               //var hasher = new PasswordHasher<User>();
               //builder.Entity<User>().HasData(new User
               //{
               //    Id = uId,
               //    UserName = "Admin",
               //    NormalizedUserName = "admin",
               //    Email = "software@caston.tv",
               //    NormalizedEmail = "software@caston.tv",
               //    EmailConfirmed = true,
               //    PasswordHash = hasher.HashPassword(null, "ySVh9AJY."),
               //    SecurityStamp = string.Empty,
               //});

               //builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
               //{
               //    RoleId = rId,
               //    UserId = uId,

               //});

          }
     }
}

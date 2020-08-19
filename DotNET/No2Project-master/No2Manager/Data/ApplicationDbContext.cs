using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using No2API.Entities.Models;

namespace No2Manager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public static string ConnectionString { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Userinfo> Userinfos { get; set; }
        public DbSet<Info> Infos { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<CouponCode> CouponCodes { get; set; }
        public DbSet<CouponUser> CouponUsers { get; set; }
    }
}

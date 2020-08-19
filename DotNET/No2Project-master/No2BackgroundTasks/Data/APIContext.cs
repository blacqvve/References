using Microsoft.EntityFrameworkCore;
using No2API.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace No2BackgroundTasks.Data
{
    public class APIContext:DbContext
    {
        public static string ConnectionString { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Userinfo> Userinfos { get; set; }
        public DbSet<Info> Infos { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<CouponCode> CouponCodes { get; set; }
        public DbSet<CouponUser> CouponUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}

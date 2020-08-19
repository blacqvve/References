using Microsoft.EntityFrameworkCore;
using No2API.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace No2Web.Data
{
    public class DataContext:DbContext
    {
        public static string ConnectionString { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionString);
        }

        public DbSet<User> Users { get; set; }
    }
}

using System.Data.Entity;

using System.Collections.Generic;

namespace AutoAdsWebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("AutoAdsDB") { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}

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

        
        public DbSet<Review> Reviews { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ad>()
                .HasRequired(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                .HasRequired(r => r.Company)
                .WithMany()
                .HasForeignKey(r => r.CompanyId);
        }
    }
}

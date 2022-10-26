using Microsoft.EntityFrameworkCore;
using webapp.model;

namespace webapp.data
{
    public class WebAppContext : DbContext
    {
        public WebAppContext(DbContextOptions<WebAppContext> options) : base(options)
        {
        }

        // set foreign keys
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // many Services to one UserProfile
            modelBuilder.Entity<Service>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(s => s.UserProfileId);


            // many orders to one service
            modelBuilder.Entity<Order>()
                .HasOne<Service>()
                .WithMany()
                .HasForeignKey(o => o.ServiceId);
            // many orders to one user
            modelBuilder.Entity<Order>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(o => o.UserProfileId);
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Order> Orders => Set<Order>();
    }

}
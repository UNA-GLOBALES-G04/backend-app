using Microsoft.EntityFrameworkCore;
using webapp.model;
using webapp.model.auth;

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

            // in order to have a user profile, the user must have a user credential
            modelBuilder.Entity<UserProfile>()
                .HasOne<UserCredential>()
                .WithOne()
                .HasForeignKey<UserProfile>(u => u.Id);
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
    }

}
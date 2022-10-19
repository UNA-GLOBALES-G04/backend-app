using Microsoft.EntityFrameworkCore;
using webapp.model;

namespace webapp.data
{
    public class WebAppContext : DbContext
    {
        public WebAppContext(DbContextOptions<WebAppContext> options) : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Service> Services => Set<Service>();
    }

}
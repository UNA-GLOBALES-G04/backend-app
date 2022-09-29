using Microsoft.EntityFrameworkCore;
using webapp.model;

namespace webapp.data
{
    public class UserProfileContext : DbContext
    {
        public UserProfileContext(DbContextOptions<UserProfileContext> options) : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    }

}
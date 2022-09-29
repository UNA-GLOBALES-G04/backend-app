using Microsoft.EntityFrameworkCore;
using webapp.data;
using webapp.model;

namespace webapp.service
{
    public class UserProfileService
    {

        private UserProfileContext context;
        public UserProfileService(UserProfileContext context)
        {
            this.context = context;
        }

        public UserProfile? getUserProfile(string userID)
        {
            return context.UserProfiles.Find(userID);
        }

        public UserProfile? getByEmail(string email)
        {
            return context.UserProfiles.FirstOrDefault(u => u.email == email);
        }

        public bool existsUserProfile(string userID)
        {
            return context.UserProfiles.AsNoTracking().Any(u => u.Id == userID);
        }

        public IEnumerable<UserProfile> getAllUserProfiles()
        {
            return context.UserProfiles.ToList();
        }

        public UserProfile? createUserProfile(UserProfile userProfile)
        {
            userProfile.profilePictureID = Guid.NewGuid().ToString();
            userProfile.isDeleted = false;
            userProfile.isVerified = false;
            context.UserProfiles.Add(userProfile);
            if (context.SaveChanges() > 0)
            {
                return userProfile;
            }
            return null;
        }

        public UserProfile updateUserProfile(UserProfile userProfile)
        {
            context.UserProfiles.Update(userProfile);
            context.SaveChanges();
            return userProfile;
        }

        public void deleteUserProfile(string userID)
        {
            var userProfile = getUserProfile(userID);
            if (userProfile != null)
            {
                context.UserProfiles.Remove(userProfile);
                context.SaveChanges();
            }
        }


    }
}
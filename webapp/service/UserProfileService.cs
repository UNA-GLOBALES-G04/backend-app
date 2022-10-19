using Microsoft.EntityFrameworkCore;
using webapp.data;
using webapp.model;

namespace webapp.service
{
    public class UserProfileService
    {

        private WebAppContext context;
        public UserProfileService(WebAppContext context)
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

        public UserProfile updateUserProfile(UserProfile userProfile, bool isUser)
        {
            // the user cannot update some fields of the profile
            if (isUser)
            {
                // get the original legalDocumentID, birthDate, isDeleted and isVerified
                var originalUserProfile = context.UserProfiles.Find(userProfile.Id);
                if (originalUserProfile != null)
                {
                    userProfile.legalDocumentID = originalUserProfile.legalDocumentID;
                    userProfile.birthDate = originalUserProfile.birthDate;
                    userProfile.isDeleted = originalUserProfile.isDeleted;
                    userProfile.isVerified = originalUserProfile.isVerified;
                }
            }
            context.UserProfiles.Update(userProfile);
            context.SaveChanges();
            return userProfile;
        }

        public bool deleteUserProfile(string userID)
        {
            var userProfile = getUserProfile(userID);
            if (userProfile != null)
            {
                userProfile.isDeleted = true;
                context.UserProfiles.Update(userProfile);
                return context.SaveChanges() > 0;
            }
            return false;
        }


    }
}
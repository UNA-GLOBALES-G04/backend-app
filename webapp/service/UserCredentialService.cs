
using System;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using webapp.data;
using webapp.model.auth;

namespace webapp.service
{

    public class UserCredentialService
    {

        private WebAppContext context;
        public UserCredentialService(WebAppContext context)
        {
            this.context = context;
        }

        public UserCredential? getUserCredential(string ID)
        {
            return context.UserCredentials.Find(ID);
        }

        public bool existsUserCredential(string ID)
        {
            return context.UserCredentials.AsNoTracking().Any(s => s.Id == ID);
        }

        public IEnumerable<UserCredential> getAllUserCredentials()
        {
            return context.UserCredentials.ToList();
        }

        public UserCredential? createUserCredential(UserCredential userCredential)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            userCredential.Id = "";
            // generate a new of 36 characters until it is unique
            while (existsUserCredential(userCredential.Id) && userCredential.Id.Length == 36)
            {
                // can be alphanumeric with lowercase
                var str = new string(Enumerable.Repeat(chars, 36)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                userCredential.Id = str;
            }
            // hash the password
            userCredential.Password = Argon2.Hash(userCredential.Password);
            context.UserCredentials.Add(userCredential);
            return context.SaveChanges() > 0 ? userCredential : null;
        }

        public UserCredential? updateUserCredential(UserCredential userCredential)
        {
            // the following IDs are the only ones that can be updated
            // email, password
            var originalUserCredential = context.UserCredentials.Find(userCredential.Id);
            if (originalUserCredential != null)
            {
                userCredential.Id = originalUserCredential.Id;
                userCredential.Password = Argon2.Hash(userCredential.Password);
                context.SaveChanges();
                return userCredential;
            }
            return null;
        }

        public bool deleteUserCredential(Guid UserCredentialID)
        {
            var UserCredential = context.UserCredentials.Find(UserCredentialID);
            if (UserCredential != null)
            {
                var result = context.UserCredentials.Remove(UserCredential);
                return context.SaveChanges() > 0;
            }
            return false;
        }

        private static Random random = new Random();

    }
}
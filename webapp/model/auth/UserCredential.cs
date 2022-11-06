using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapp.model.auth
{
    [Index(nameof(Email), IsUnique = true)]
    public class UserCredential
    {

        [MaxLength(36)]
        public string Id { get; set; } = "";
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public UserCredential(string id, string email, string password)
        {
            Id = id;
            Email = email;
            Password = password;
        }
    }
}

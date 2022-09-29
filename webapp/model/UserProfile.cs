using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapp.model
{
    [Index(nameof(email), IsUnique = true)]
    [Index(nameof(legalDocumentID), IsUnique = true)]
    [Index(nameof(profilePictureID), IsUnique = true)]
    public class UserProfile
    {
        [MaxLength(36)]
        public string Id { get; set; } = "";
        public string fullName { get; set; } = "";
        public string legalDocumentID { get; set; } = "";
        public string email { get; set; } = "";
        public DateTime birthDate { get; set; } = DateTime.Now;
        public string? address { get; set; } = "";
        public string profilePictureID { get; set; } = "";
        public bool isDeleted { get; set; } = false;
        public bool isVerified { get; set; } = false;

        ICollection<Service> services { get; set; } = new List<Service>();

        public UserProfile()
        {
        }

        public UserProfile(
            string userID,
            string fullName,
            string legalDocumentID,
            string email,
            DateTime birthDate,
            string? address,
            string profilePictureID,
            bool isDeleted,
            bool isVerified,
            ICollection<Service> services)
        {
            this.Id = userID;
            this.fullName = fullName;
            this.legalDocumentID = legalDocumentID;
            this.email = email;
            this.birthDate = birthDate;
            this.address = address;
            this.profilePictureID = profilePictureID;
            this.isDeleted = isDeleted;
            this.isVerified = isVerified;
            this.services = services;
        }
    }
}
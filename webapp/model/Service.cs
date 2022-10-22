using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.model
{
    public class Service
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string serviceName { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string[] tags { get; set; }
        public string[] multimedia { get; set; }
        public bool isDeleted { get; set; }

        public Service(
            Guid Id,
            string serviceName,
            string description,
            string email,
            string phoneNumber,
            string[] tags,
            string[] multimedia,
            bool isDeleted)
        {
            this.Id = Id;
            this.serviceName = serviceName;
            this.description = description;
            this.email = email;
            this.phoneNumber = phoneNumber;
            this.tags = tags;
            this.multimedia = multimedia;
            this.isDeleted = isDeleted;
        }

        [MaxLength(36)]
        public string UserProfileId { get; set; } = "";
        public UserProfile UserProfile { get; set; } = new UserProfile();


    }
}
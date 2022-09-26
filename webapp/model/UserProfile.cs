namespace webapp.model
{
    public class UserProfile
    {
        public string userID { get; set; }
        public string fullName { get; set; }
        public string legalDocumentID { get; set; }
        public string email { get; set; }
        public DateTime birthDate { get; set; }
        public string? address { get; set; }
        public string profilePictureID { get; set; }
        public bool isDeleted { get; set; }
        public bool isVerified { get; set; }

        ICollection<Service> services { get; set; }

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
            this.userID = userID;
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
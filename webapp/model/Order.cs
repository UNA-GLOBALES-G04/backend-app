
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.model
{
    public class Order
    {
        public enum OrderStatus
        {
            PENDING,
            ACCEPTED,
            REJECTED,
            COMPLETED,
            CANCELLED
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Service")]
        public Guid ServiceId { get; set; }

        [MaxLength(36)]
        [ForeignKey("UserProfile")]
        public string UserProfileId { get; set; }

        public DateTime requiredDate { get; set; }

        public string direction { get; set; }

        public OrderStatus current_status { get; set; }

        public int? rating { get; set; }
        public string? description { get; set; }

        public Order(
            Guid Id,
            Guid ServiceId,
            string UserProfileId,
            DateTime requiredDate,
            string direction,
            OrderStatus current_status,
            int? rating,
            string? description)
        {
            this.Id = Id;
            this.ServiceId = ServiceId;
            this.UserProfileId = UserProfileId;
            this.requiredDate = requiredDate;
            this.direction = direction;
            this.current_status = current_status;
            this.rating = rating;
            this.description = description;
        }
    }
}
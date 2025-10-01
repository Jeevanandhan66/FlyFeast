using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Refund
    {
        [Key]
        public int RefundId { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("Initiated|Processed|Failed")]
        public string Status { get; set; } = "Initiated";

        [Required]
        public string ProcessedById { get; set; } = string.Empty;

        [ForeignKey(nameof(ProcessedById))]
        public ApplicationUser? ProcessedUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

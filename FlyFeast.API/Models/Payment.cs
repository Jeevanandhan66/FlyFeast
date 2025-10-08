using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string? Provider { get; set; }

        [StringLength(100)]
        public string? ProviderRef { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("Initiated|Success|Failed")]
        public string Status { get; set; } = "Initiated";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

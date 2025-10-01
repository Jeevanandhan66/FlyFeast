using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public int ScheduleId { get; set; }
        [ForeignKey(nameof(ScheduleId))]
        public Schedule? Schedule { get; set; }

        [Required, StringLength(50)]
        public string BookingRef { get; set; } = string.Empty;

        [Required, Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("Pending|Confirmed|Cancelled|Refunded")]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BookingItem>? BookingItems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Refund>? Refunds { get; set; }
        public ICollection<BookingCancellation>? BookingCancellations { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        public int ScheduleId { get; set; }
        [ForeignKey(nameof(ScheduleId))]
        public Schedule? Schedule { get; set; }

        [Required, StringLength(50)]
        public string BookingRef { get; set; } = string.Empty;

        [Required, Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<BookingItem>? BookingItems { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Refund>? Refunds { get; set; }
        public ICollection<BookingCancellation>? BookingCancellations { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Refunded
    }
}

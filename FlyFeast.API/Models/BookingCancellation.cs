using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class BookingCancellation
    {
        [Key]
        public int CancellationId { get; set; }
        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [Required]
        public string CancelledById { get; set; } = string.Empty;

        [ForeignKey(nameof(CancelledById))]
        public ApplicationUser? CancelledUser { get; set; }

        [StringLength(255)]
        public string? Reason { get; set; }

        public DateTime CancelledAt { get; set; } = DateTime.Now;
    }
}

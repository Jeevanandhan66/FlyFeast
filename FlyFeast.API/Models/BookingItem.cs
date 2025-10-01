using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class BookingItem
    {
        [Key]
        public int BookingItemId { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        public int SeatId { get; set; }

        [ForeignKey(nameof(SeatId))]
        public Seat? Seat { get; set; }

        public int PassengerId { get; set; }

        [ForeignKey(nameof(PassengerId))]
        public Passenger? Passenger { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal PriceAtBooking { get; set; }
    }
}

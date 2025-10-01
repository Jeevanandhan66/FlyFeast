using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public int RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public FlightRoute? Route { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [NotMapped]
        public int Duration => (int)(ArrivalTime - DepartureTime).TotalMinutes;

        [Required, Range(1, int.MaxValue)]
        public int SeatCapacity { get; set; }

        public int? AvailableSeats { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("Scheduled|Delayed|Cancelled|Completed",
            ErrorMessage = "Status must be one of: Scheduled, Delayed, Cancelled, Completed")]
        public string Status { get; set; } = "Scheduled";

        public ICollection<Seat>? Seats { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int RouteId { get; set; }

        [ForeignKey(nameof(RouteId))]
        public FlightRoute? Route { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [NotMapped]
        public TimeSpan Duration => ArrivalTime - DepartureTime;

        [Required, Range(1, int.MaxValue)]
        public int SeatCapacity { get; set; }

        public int? AvailableSeats { get; set; }

        [Required]
        public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

        // Navigation properties
        public ICollection<Seat>? Seats { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
    public enum ScheduleStatus
    {
        Scheduled,
        Delayed,
        Cancelled,
        Completed
    }
}

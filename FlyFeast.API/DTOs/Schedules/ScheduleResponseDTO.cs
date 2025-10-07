using FlyFeast.API.DTOs.Routes;
using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Schedules
{
    public class ScheduleResponseDTO
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public RouteResponseDTO Route { get; set; } = new();

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        [Required]
        public string DurationFormatted { get; set; } = string.Empty;

        [Required]
        public int SeatCapacity { get; set; }

        public int? AvailableSeats { get; set; }

        [Required]
        public string Status { get; set; } = "Scheduled";
    }
}

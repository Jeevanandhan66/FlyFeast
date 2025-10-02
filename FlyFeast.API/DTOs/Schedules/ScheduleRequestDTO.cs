using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Schedules
{
    public class ScheduleRequestDTO
    {
        [Required(ErrorMessage = "RouteId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RouteId must be a valid positive number.")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "DepartureTime is required.")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "ArrivalTime is required.")]
        public DateTime ArrivalTime { get; set; }

        [Required(ErrorMessage = "SeatCapacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SeatCapacity must be greater than 0.")]
        public int SeatCapacity { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Scheduled|Delayed|Cancelled|Completed",
            ErrorMessage = "Status must be one of: Scheduled, Delayed, Cancelled, Completed")]
        public string Status { get; set; } = "Scheduled";
    }
}

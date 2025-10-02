using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Schedules
{
    public class ScheduleSummaryDTO
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }


        [Required]
        public int DurationMinutes { get; set; }

        [Required]
        public string DurationFormatted { get; set; } = string.Empty;

        public int? AvailableSeats { get; set; }

        [Required]
        public string Status { get; set; } = "Scheduled";
    }
}

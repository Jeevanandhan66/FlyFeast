using System.ComponentModel.DataAnnotations;
using FlyFeast.API.DTOs.Schedules;

namespace FlyFeast.API.DTOs.Seats
{
    public class SeatResponseDTO
    {
        [Required]
        public int SeatId { get; set; }

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Economy|Business|First")]
        public string Class { get; set; } = "Economy";

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsBooked { get; set; }

        [Required]
        public ScheduleSummaryDTO Schedule { get; set; } = new();
    }
}

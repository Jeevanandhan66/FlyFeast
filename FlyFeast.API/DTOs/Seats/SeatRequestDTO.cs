using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Seats
{
    public class SeatRequestDTO
    {
        [Required(ErrorMessage = "ScheduleId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ScheduleId must be a valid positive number.")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "SeatNumber is required.")]
        [StringLength(10, ErrorMessage = "SeatNumber cannot exceed 10 characters.")]
        public string SeatNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seat class is required.")]
        [RegularExpression("Economy|Business|First",
            ErrorMessage = "Seat class must be one of: Economy, Business, First")]
        public string Class { get; set; } = "Economy";

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Bookings
{
    public class BookingRequestDTO
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ScheduleId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ScheduleId must be greater than 0.")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "TotalAmount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "TotalAmount must be non-negative.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "At least one seat must be booked.")]
        [MinLength(1, ErrorMessage = "Booking must include at least one seat.")]
        public List<SeatPassengerDTO> Seats { get; set; } = new();

        public string Status { get; set; } = "Pending";
    }
}
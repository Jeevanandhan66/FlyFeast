using FlyFeast.API.DTOs.Aircraft_Airport;
using FlyFeast.API.DTOs.Bookings;

namespace FlyFeast.API.DTOs.Refunds
{
    public class RefundResponseDTO
    {
        public int RefundId { get; set; }
        public BookingResponseDTO Booking { get; set; } = new();
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Initiated";
        public UserSummaryDTO ProcessedUser { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

}

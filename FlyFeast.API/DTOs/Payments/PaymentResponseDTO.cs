using FlyFeast.API.DTOs.Bookings;

namespace FlyFeast.API.DTOs.Payments
{
    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public BookingResponseDTO Booking { get; set; } = new();
        public decimal Amount { get; set; }
        public string? Provider { get; set; }
        public string? ProviderRef { get; set; }
        public string Status { get; set; } = "Initiated";
        public DateTime CreatedAt { get; set; }
    }
}

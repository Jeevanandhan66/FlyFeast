namespace FlyFeast.API.DTOs.Payments
{
    public class PaymentRequestDTO
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string? Provider { get; set; }

        public string UserId { get; set; } = string.Empty;
    }

}

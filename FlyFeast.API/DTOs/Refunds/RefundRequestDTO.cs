namespace FlyFeast.API.DTOs.Refunds
{
    public class RefundRequestDTO
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string? ProcessedById { get; set; }
        public string Status { get; set; } = "Initiated";
    }


}

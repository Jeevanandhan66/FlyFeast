namespace FlyFeast.API.DTOs.Bookings
{
    public class BookingCancellationDTO
    {
        public int BookingId { get; set; }
        public string CancelledById { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime CancelledAt { get; set; }
    }

}

namespace FlyFeast.API.DTOs.Seats
{
    public class SeatResponseDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Class { get; set; } = "Economy";
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
        public ScheduleSummaryDTO Schedule { get; set; } = new();
    }
}

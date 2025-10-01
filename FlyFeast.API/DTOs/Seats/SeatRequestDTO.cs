namespace FlyFeast.API.DTOs.Seats
{
    public class SeatRequestDTO
    {
        public int ScheduleId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Class { get; set; } = "Economy";
        public decimal Price { get; set; }
    }
}

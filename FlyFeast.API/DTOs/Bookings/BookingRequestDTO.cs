namespace FlyFeast.API.DTOs.Bookings
{
    public class BookingRequestDTO
    {
        public string UserId { get; set; } = string.Empty;
        public int ScheduleId { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SeatPassengerDTO> Seats { get; set; } = new();

        public string Status { get; set; } = "Pending";
    }

}

namespace FlyFeast.API.DTOs.Schedules
{
    public class ScheduleRequestDTO
    {
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int SeatCapacity { get; set; }
        public string Status { get; set; } = "Scheduled";
    }

}

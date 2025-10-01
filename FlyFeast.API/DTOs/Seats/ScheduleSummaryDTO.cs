namespace FlyFeast.API.DTOs.Seats
{
    public class ScheduleSummaryDTO
    {
        public int ScheduleId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}

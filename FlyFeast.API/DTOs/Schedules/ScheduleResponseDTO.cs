using FlyFeast.API.DTOs.Routes;

namespace FlyFeast.API.DTOs.Schedules
{
    public class ScheduleResponseDTO
    {
        public int ScheduleId { get; set; }
        public RouteResponseDTO Route { get; set; } = new();
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Duration => (int)(ArrivalTime - DepartureTime).TotalMinutes;
        public int SeatCapacity { get; set; }
        public int? AvailableSeats { get; set; }
        public string Status { get; set; } = "Scheduled";
    }

}

namespace FlyFeast.API.DTOs.Aircraft_Airport
{
    public class AircraftRequestDTO
    {
        public string AircraftCode { get; set; } = string.Empty;
        public string? AircraftName { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
    }

}

namespace FlyFeast.API.DTOs.Aircraft_Airport
{
    public class AircraftResponseDTO
    {
        public int AircraftId { get; set; }
        public string AircraftCode { get; set; } = string.Empty;
        public string? AircraftName { get; set; }
        public UserSummaryDTO Owner { get; set; } = new();
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
    }

}

namespace FlyFeast.API.DTOs.Aircraft_Airport
{
    public class AirportDTO
    {
        public int AirportId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}

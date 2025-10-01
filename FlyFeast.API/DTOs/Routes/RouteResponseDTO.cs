using FlyFeast.API.DTOs.Aircraft_Airport;

namespace FlyFeast.API.DTOs.Routes
{
    public class RouteResponseDTO
    {
        public int RouteId { get; set; }
        public AircraftResponseDTO Aircraft { get; set; } = new();
        public AirportDTO OriginAirport { get; set; } = new();
        public AirportDTO DestinationAirport { get; set; } = new();
        public decimal BaseFare { get; set; }
    }

}

namespace FlyFeast.API.DTOs.Routes
{
    public class RouteRequestDTO
    {
        public int AircraftId { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public decimal BaseFare { get; set; }
    }

}

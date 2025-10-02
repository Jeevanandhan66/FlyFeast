using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Routes
{
    public class RouteRequestDTO
    {
        [Required(ErrorMessage = "AircraftId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "AircraftId must be a valid positive number.")]
        public int AircraftId { get; set; }

        [Required(ErrorMessage = "OriginAirportId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "OriginAirportId must be a valid positive number.")]
        public int OriginAirportId { get; set; }

        [Required(ErrorMessage = "DestinationAirportId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "DestinationAirportId must be a valid positive number.")]
        public int DestinationAirportId { get; set; }

        [Required(ErrorMessage = "BaseFare is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "BaseFare must be greater than 0.")]
        public decimal BaseFare { get; set; }
    }
}

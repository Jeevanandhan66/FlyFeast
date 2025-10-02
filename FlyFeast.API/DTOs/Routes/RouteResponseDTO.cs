using System.ComponentModel.DataAnnotations;
using FlyFeast.API.DTOs.Aircraft_Airport;

namespace FlyFeast.API.DTOs.Routes
{
    public class RouteResponseDTO
    {
        [Required]
        public int RouteId { get; set; }

        [Required]
        public AircraftResponseDTO Aircraft { get; set; } = new();

        [Required]
        public AirportDTO OriginAirport { get; set; } = new();

        [Required]
        public AirportDTO DestinationAirport { get; set; } = new();

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "BaseFare must be greater than 0.")]
        public decimal BaseFare { get; set; }
    }
}

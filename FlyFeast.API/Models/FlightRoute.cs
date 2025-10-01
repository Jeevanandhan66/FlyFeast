using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class FlightRoute
    {
        [Key]
        public int RouteId { get; set; }

        public int AircraftId { get; set; }

        [ForeignKey(nameof(AircraftId))]
        public Aircraft? Aircraft { get; set; }

        public int OriginAirportId { get; set; }

        [ForeignKey(nameof(OriginAirportId))]
        public Airport? OriginAirport { get; set; }

        public int DestinationAirportId { get; set; }

        [ForeignKey(nameof(DestinationAirportId))]
        public Airport? DestinationAirport { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal BaseFare { get; set; }

        public ICollection<Schedule>? Schedules { get; set; }
    }
}

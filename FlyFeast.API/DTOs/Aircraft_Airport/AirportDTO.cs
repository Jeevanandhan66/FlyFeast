using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Aircraft_Airport
{
    public class AirportDTO
    {
        public int AirportId { get; set; }
        [Required]
        public string AirportName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}

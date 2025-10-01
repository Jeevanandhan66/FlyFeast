using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.Models
{
    public class Airport
    {
        [Key]
        public int AirportId { get; set; }

        [Required, StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Country { get; set; } = string.Empty;
    }
}

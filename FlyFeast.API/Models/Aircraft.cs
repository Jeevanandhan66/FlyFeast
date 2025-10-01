using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Aircraft
    {
        [Key]
        public int AircraftId { get; set; }

        [Required, StringLength(20)]
        public string AircraftCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AircraftName { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey(nameof(OwnerId))]
        public ApplicationUser? Owner { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int EconomySeats { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int BusinessSeats { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int FirstClassSeats { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FlightRoute>? Routes { get; set; }
    }
}

//test comment
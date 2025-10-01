using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }
        public int ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public Schedule? Schedule { get; set; }

        [Required, StringLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        [Required, StringLength(20)]
        [RegularExpression("Economy|Business|First")]
        public string Class { get; set; } = "Economy";

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsBooked { get; set; } = false;
    }
}

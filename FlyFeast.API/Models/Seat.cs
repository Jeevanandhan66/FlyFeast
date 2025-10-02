using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyFeast.API.Models
{
    public class Seat
    {
        [Key]
        public int SeatId { get; set; }

        [Required]
        public int ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public Schedule? Schedule { get; set; }

        [Required, StringLength(20)]
        public string SeatNumber { get; set; } = string.Empty;

        [Required]
        public SeatClass Class { get; set; } = SeatClass.Economy;

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsBooked { get; set; } = false;
    }

    public enum SeatClass
    {
        Economy,
        Business,
        First
    }
}

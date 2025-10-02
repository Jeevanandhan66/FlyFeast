using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Bookings
{
    public class SeatPassengerDTO
    {
        [Required(ErrorMessage = "SeatId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SeatId must be greater than 0.")]
        public int SeatId { get; set; }

        [Required(ErrorMessage = "PassengerId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PassengerId must be greater than 0.")]
        public int PassengerId { get; set; }
    }
}

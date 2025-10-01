using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.User_Role
{
    public class PassengerRequestDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? PassportNumber { get; set; }

        [StringLength(50)]
        public string? Nationality { get; set; }
    }
}

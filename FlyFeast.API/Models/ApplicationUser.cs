using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public Passenger? Passenger { get; set; }

        [StringLength(10)]
        [RegularExpression("Male|Female|Other")]
        public string? Gender { get; set; }
        [StringLength(10)]
        public string? PhoneNumber { get; set; }


        [StringLength(255)]
        public string? Address { get; set; }


        public bool IsActive { get; set; } = true;

        public ICollection<Aircraft>? OwnedAircrafts { get; set; }
    }
}

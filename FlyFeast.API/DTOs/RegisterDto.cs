using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name must not exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Passport number is required")]
        [StringLength(20, ErrorMessage = "Passport number must not exceed 20 characters")]
        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Nationality is required")]
        [StringLength(50, ErrorMessage = "Nationality must not exceed 50 characters")]
        public string Nationality { get; set; }
    }
}

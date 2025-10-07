using System.ComponentModel.DataAnnotations;

namespace FlyFeast.API.DTOs.Payments
{
    public class PaymentRequestDTO
    {
        public int BookingId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be >= 0.")]
        public decimal Amount { get; set; }

        public string? Provider { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Initiated|Success|Failed", ErrorMessage = "Status must be Initiated, Success, or Failed.")]
        public string Status { get; set; } = "Initiated";
    }
}

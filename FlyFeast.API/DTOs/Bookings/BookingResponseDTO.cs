using FlyFeast.API.DTOs.Aircraft_Airport;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.DTOs.Schedules;

public class BookingResponseDTO
{
    public int BookingId { get; set; }
    public string BookingRef { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserSummaryDTO User { get; set; } = new();
    public ScheduleSummaryDTO Schedule { get; set; } = new();
    public List<BookingItemDTO> BookingItems { get; set; } = new();
}

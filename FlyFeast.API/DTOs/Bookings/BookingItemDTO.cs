using FlyFeast.API.DTOs.Seats;
using FlyFeast.API.DTOs.User_Role;

namespace FlyFeast.API.DTOs.Bookings
{
    public class BookingItemDTO
    {
        public int BookingItemId { get; set; }
        public SeatResponseDTO Seat { get; set; } = new();
        public PassengerDTO Passenger { get; set; } = new();
        public decimal PriceAtBooking { get; set; }
    }

}

using FlyFeast.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetByUserIdAsync(string userId);

        Task<Booking> AddAsync(Booking booking);

        Task<Booking> AddAsync(Booking booking, List<(int SeatId, int PassengerId)> seatPassengerPairs);

        Task<Booking?> UpdateAsync(int id, Booking booking);
        Task<bool> DeleteAsync(int id);

        Task AddCancellationAsync(BookingCancellation cancellation);

        Task ReleaseSeatsAsync(int bookingId);
    }
}

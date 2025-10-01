using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser> AddAsync(ApplicationUser user, string password);
        Task<List<ApplicationUser>> GetAllAsync();
        Task<bool> ExistsByEmailAsync(string email);

        // Bookings
        Task<List<Booking>> GetBookingsAsync(string userId);
        Task<Booking> BookFlightAsync(Booking booking);
    }
}

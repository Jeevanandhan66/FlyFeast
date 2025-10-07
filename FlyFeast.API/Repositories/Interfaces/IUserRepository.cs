using FlyFeast.API.Models;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user);
        Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    }
}

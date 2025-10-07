using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FlyFeast.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? user : null;
        }

        public async Task<bool> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
    }
}

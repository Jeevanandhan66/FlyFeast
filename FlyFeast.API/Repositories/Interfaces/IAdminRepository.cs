using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;

namespace FlyFeast.API.Data.Repositories
{
    public interface IAdminRepository
    {
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password, IEnumerable<string> roles);
        Task<IdentityResult> AssignRoleAsync(string userId, string roleName);
        Task<List<IdentityRole>> GetRolesAsync();
        Task<IdentityRole> AddRoleAsync(string roleName);

        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> ToggleUserActiveAsync(string userId, bool isActive);
    }
}


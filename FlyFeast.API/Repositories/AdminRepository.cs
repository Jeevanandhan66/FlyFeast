using FlyFeast.API.Data;
using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _userManager.Users
                .Include(u => u.Passenger)
                .ToListAsync();
        }

        public async Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password, IEnumerable<string> roles)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        public async Task<IdentityResult> AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole> AddRoleAsync(string roleName)
        {
            var role = new IdentityRole(roleName);
            await _roleManager.CreateAsync(role);
            return role;
        }

        // ---------------- NEW METHODS ----------------

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users
                .Include(u => u.Passenger)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser updatedUser)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id);
            if (user == null) return null;

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.UserName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Gender = updatedUser.Gender;
            user.Address = updatedUser.Address;
            user.IsActive = updatedUser.IsActive;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? user : null;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ToggleUserActiveAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}

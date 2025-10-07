using FlyFeast.API.DTOs.User_Role;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer,Admin,Manager")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET profile
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.UserName,
                user.PhoneNumber,
                user.Gender,
                user.Address,
                user.IsActive
            });
        }

        // UPDATE profile
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserRequestDTO dto)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            user.FullName = dto.FullName;
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.Phone;
            user.Gender = dto.Gender;
            user.Address = dto.Address;

            var updated = await _userRepository.UpdateUserAsync(user);
            if (updated == null) return StatusCode(500, "Could not update profile");

            return Ok(new { message = "Profile updated successfully" });
        }

        // CHANGE password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromQuery] string currentPassword, [FromQuery] string newPassword)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var result = await _userRepository.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result) return BadRequest("Password change failed");

            return Ok(new { message = "Password changed successfully" });
        }
    }
}

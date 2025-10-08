using AutoMapper;
using FlyFeast.API.Data.Repositories;
using FlyFeast.API.DTOs;
using FlyFeast.API.DTOs.Payments;
using FlyFeast.API.DTOs.Refunds;
using FlyFeast.API.DTOs.User_Role;
using FlyFeast.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Customer")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AdminController(
            IAdminRepository adminRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _adminRepository = adminRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        

        [HttpGet("users")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _adminRepository.GetUsersAsync();
                var userDtos = new List<UserResponseDTO>();

                foreach (var user in users)
                {
                    var dto = _mapper.Map<UserResponseDTO>(user);
                    var roles = await _userManager.GetRolesAsync(user);
                    dto.Roles = roles.Select(r => new RoleDTO { Id = r, Name = r }).ToList();
                    userDtos.Add(dto);
                }

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("admin/users")]
        public async Task<IActionResult> CreateAdminUser([FromBody] UserRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.Phone,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                if (dto.RoleNames != null && dto.RoleNames.Any())
                {
                    var validRoles = new List<string>();
                    foreach (var roleName in dto.RoleNames)
                    {
                        var roleExists = await _roleManager.RoleExistsAsync(roleName);
                        if (roleExists) validRoles.Add(roleName);
                    }
                    if (validRoles.Any())
                        await _userManager.AddToRolesAsync(user, validRoles);
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var rolesList = new List<RoleDTO>();
                foreach (var roleName in userRoles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        rolesList.Add(new RoleDTO { Id = role.Id, Name = role.Name });
                    }
                }

                var response = new UserResponseDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Gender = user.Gender,
                    Address = user.Address,
                    IsActive = user.IsActive,
                    Roles = rolesList,
                    Passengers = new List<PassengerDTO>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("users/{userId}/roles/{roleName}")]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            try
            {
                var result = await _adminRepository.AssignRoleAsync(userId, roleName);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));

                return Ok(new { UserId = userId, RoleName = roleName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("roles")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _adminRepository.GetRolesAsync();
                var roleDtos = roles.Select(r => new RoleDTO { Id = r.Id, Name = r.Name }).ToList();
                return Ok(roleDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromBody] RoleDTO dto)
        {
            try
            {
                var role = await _adminRepository.AddRoleAsync(dto.Name);
                return Ok(new RoleDTO { Id = role.Id, Name = role.Name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("users/{userId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            return Ok(_mapper.Map<UserResponseDTO>(user));
        }

        [HttpPut("users/{userId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserRequestDTO dto)
        {
            var existing = await _adminRepository.GetUserByIdAsync(userId);
            if (existing == null) return NotFound("User not found.");

            existing.FullName = dto.FullName;
            existing.UserName = dto.UserName;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.Phone;
            existing.Gender = dto.Gender;
            existing.Address = dto.Address;

            var updated = await _adminRepository.UpdateUserAsync(existing);
            if (updated == null) return StatusCode(500, "Could not update user.");
            return Ok(_mapper.Map<UserResponseDTO>(updated));
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var success = await _adminRepository.DeleteUserAsync(userId);
            if (!success) return NotFound("User not found.");
            return NoContent();
        }

        [HttpPut("users/{userId}/activate")]
        public async Task<IActionResult> ToggleUserActive(string userId, [FromQuery] bool isActive)
        {
            var success = await _adminRepository.ToggleUserActiveAsync(userId, isActive);
            if (!success) return NotFound("User not found.");
            return Ok(new { userId, isActive });
        }

    }
}

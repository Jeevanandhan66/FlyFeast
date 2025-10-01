using FlyFeast.API.Configuration;
using FlyFeast.API.Data;
using FlyFeast.API.DTOs;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            try
            {
                if (dto.Password != dto.ConfirmPassword)
                    return BadRequest("Passwords do not match");

                // Create base ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));


                await _userManager.AddToRoleAsync(user, "Customer");

                var passenger = new Passenger
                {
                    UserId = user.Id,
                    DateOfBirth = null,
                    PassportNumber = null,
                    Nationality = null
                };

                _context.Passengers.Add(passenger);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Customer registration successful. Please login." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null) return Unauthorized("Invalid credentials");

                var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!passwordValid) return Unauthorized("Invalid credentials");

                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user, roles);

                var response = new AuthResponseDTO
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                    Role = roles.FirstOrDefault() ?? "Customer",
                    FullName = user.FullName
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

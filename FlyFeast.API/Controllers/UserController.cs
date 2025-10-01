using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.DTOs.User_Role;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequestDTO userDto)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(userDto.Email) != null)
                    return BadRequest(new { error = "Email already exists." });

                var appUser = _mapper.Map<ApplicationUser>(userDto);
                var result = await _userManager.CreateAsync(appUser, userDto.Password);

                if (!result.Succeeded)
                    return BadRequest(new { error = string.Join("; ", result.Errors.Select(e => e.Description)) });

                var response = _mapper.Map<UserResponseDTO>(appUser);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not register user: {ex.Message}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var dtos = users.Select(u => _mapper.Map<UserResponseDTO>(u)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not retrieve users: {ex.Message}" });
            }
        }


        [HttpPost("{userId}/bookings")]
        public async Task<IActionResult> BookFlight(string userId, [FromBody] BookingRequestDTO bookingDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound(new { error = "User not found." });

                var booking = _mapper.Map<Booking>(bookingDto);
                booking.UserId = user.Id;
                booking.Status = "Confirmed";
                booking.CreatedAt = DateTime.UtcNow;

                var created = await _userRepository.BookFlightAsync(booking);
                return Ok(_mapper.Map<BookingResponseDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not book flight: {ex.Message}" });
            }
        }


        [HttpGet("{userId}/bookings")]
        public async Task<IActionResult> GetUserBookings(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound(new { error = "User not found." });

                var bookings = await _userRepository.GetBookingsAsync(user.Id);
                if (!bookings.Any()) return NotFound(new { error = "No bookings found for this user." });

                return Ok(_mapper.Map<List<BookingResponseDTO>>(bookings));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not retrieve bookings: {ex.Message}" });
            }
        }
    }
}

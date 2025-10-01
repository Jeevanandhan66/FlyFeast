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
    [Authorize(Roles = "Admin")]
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

 
        [HttpGet("aircrafts")]
        public async Task<IActionResult> GetAircrafts()
        {
            try
            {
                var aircrafts = await _adminRepository.GetAircraftsAsync();
                return Ok(_mapper.Map<List<AircraftResponseDTO>>(aircrafts));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("aircrafts")]
        public async Task<IActionResult> AddAircraft([FromBody] AircraftRequestDTO dto)
        {
            try
            {
                var aircraft = _mapper.Map<Aircraft>(dto);
                var result = await _adminRepository.AddAircraftAsync(aircraft);
                return Ok(_mapper.Map<AircraftResponseDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("airports")]
        public async Task<IActionResult> GetAirports()
        {
            try
            {
                var airports = await _adminRepository.GetAirportsAsync();
                return Ok(_mapper.Map<List<AirportDTO>>(airports));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("airports")]
        public async Task<IActionResult> AddAirport([FromBody] AirportDTO dto)
        {
            try
            {
                var airport = _mapper.Map<Airport>(dto);
                var result = await _adminRepository.AddAirportAsync(airport);
                return Ok(_mapper.Map<AirportDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("routes")]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                var routes = await _adminRepository.GetRoutesAsync();
                return Ok(_mapper.Map<List<RouteResponseDTO>>(routes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("routes")]
        public async Task<IActionResult> AddRoute([FromBody] RouteRequestDTO dto)
        {
            try
            {
                var route = _mapper.Map<FlightRoute>(dto);
                var result = await _adminRepository.AddRouteAsync(route);
                return Ok(_mapper.Map<RouteResponseDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("schedules")]
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                var schedules = await _adminRepository.GetSchedulesAsync();
                return Ok(_mapper.Map<List<ScheduleResponseDTO>>(schedules));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("schedules")]
        public async Task<IActionResult> AddSchedule([FromBody] ScheduleRequestDTO dto)
        {
            try
            {
                var schedule = _mapper.Map<Schedule>(dto);
                var result = await _adminRepository.AddScheduleAsync(schedule);
                return Ok(_mapper.Map<ScheduleResponseDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("seats")]
        public async Task<IActionResult> GetSeats()
        {
            try
            {
                var seats = await _adminRepository.GetSeatsAsync();
                return Ok(_mapper.Map<List<SeatResponseDTO>>(seats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("seats")]
        public async Task<IActionResult> AddSeat([FromBody] SeatRequestDTO dto)
        {
            try
            {
                var seat = _mapper.Map<Seat>(dto);
                var result = await _adminRepository.AddSeatAsync(seat);
                return Ok(_mapper.Map<SeatResponseDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _adminRepository.GetBookingsAsync();
                return Ok(_mapper.Map<List<BookingResponseDTO>>(bookings));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            try
            {
                var payments = await _adminRepository.GetPaymentsAsync();
                return Ok(_mapper.Map<List<PaymentResponseDTO>>(payments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("payments")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentRequestDTO dto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(dto);
                var result = await _adminRepository.AddPaymentAsync(payment);
                return Ok(_mapper.Map<PaymentResponseDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("refunds")]
        public async Task<IActionResult> GetRefunds()
        {
            try
            {
                var refunds = await _adminRepository.GetRefundsAsync();
                return Ok(_mapper.Map<List<RefundResponseDTO>>(refunds));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("refunds")]
        public async Task<IActionResult> AddRefund([FromBody] RefundRequestDTO dto)
        {
            try
            {
                var refund = _mapper.Map<Refund>(dto);
                var result = await _adminRepository.AddRefundAsync(refund);
                return Ok(_mapper.Map<RefundResponseDTO>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

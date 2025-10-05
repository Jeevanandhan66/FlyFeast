using AutoMapper;
using FlyFeast.API.DTOs.Seats;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,Customer")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IMapper _mapper;

        public SeatController(ISeatRepository seatRepository, IMapper mapper)
        {
            _seatRepository = seatRepository;
            _mapper = mapper;
        }

        // ---------------- GET ALL ----------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeats()
        {
            try
            {
                var seats = await _seatRepository.GetAllAsync();
                return Ok(_mapper.Map<List<SeatResponseDTO>>(seats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        // ---------------- GET BY ID ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeat(int id)
        {
            try
            {
                var seat = await _seatRepository.GetByIdAsync(id);
                if (seat == null) return NotFound(new { error = $"Seat with ID {id} not found." });

                return Ok(_mapper.Map<SeatResponseDTO>(seat));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        // ---------------- GET BY SCHEDULE ----------------
        [HttpGet("byschedule/{scheduleId}")]
        public async Task<IActionResult> GetSeatsBySchedule(int scheduleId)
        {
            try
            {
                var seats = await _seatRepository.GetByScheduleAsync(scheduleId);
                return Ok(_mapper.Map<List<SeatResponseDTO>>(seats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        // ---------------- CREATE ----------------
        [HttpPost]
        public async Task<IActionResult> CreateSeat([FromBody] SeatRequestDTO seatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var seat = _mapper.Map<Seat>(seatDto);
                var created = await _seatRepository.AddAsync(seat);

                return CreatedAtAction(nameof(GetSeat), new { id = created.SeatId },
                                       _mapper.Map<SeatResponseDTO>(created));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message }); // business rule violation
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not create seat: {ex.Message}" });
            }
        }

        // ---------------- UPDATE ----------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeat(int id, [FromBody] SeatRequestDTO seatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var seat = _mapper.Map<Seat>(seatDto);
                var updated = await _seatRepository.UpdateAsync(id, seat);

                if (updated == null) return NotFound(new { error = $"Seat with ID {id} not found." });

                return Ok(_mapper.Map<SeatResponseDTO>(updated));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not update seat: {ex.Message}" });
            }
        }

        // ---------------- DELETE ----------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            try
            {
                var success = await _seatRepository.DeleteAsync(id);
                if (!success) return NotFound(new { error = $"Seat with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not delete seat: {ex.Message}" });
            }
        }
    }
}

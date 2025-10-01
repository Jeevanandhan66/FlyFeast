using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.DTOs.Seats;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IMapper _mapper;

        public SeatController(ISeatRepository seatRepository, IMapper mapper)
        {
            _seatRepository = seatRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSeats()
        {
            try
            {
                var seats = await _seatRepository.GetAllAsync();
                return Ok(_mapper.Map<List<SeatResponseDTO>>(seats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeat(int id)
        {
            try
            {
                var seat = await _seatRepository.GetByIdAsync(id);
                if (seat == null) return NotFound();

                return Ok(_mapper.Map<SeatResponseDTO>(seat));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

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
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeat(SeatRequestDTO seatDto)
        {
            try
            {
                var seat = _mapper.Map<Seat>(seatDto);
                var created = await _seatRepository.AddAsync(seat);

           
                await RecalculateAvailableSeats(seat.ScheduleId);

                return CreatedAtAction(nameof(GetSeat), new { id = created.SeatId }, _mapper.Map<SeatResponseDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeat(int id, SeatRequestDTO seatDto)
        {
            try
            {
                var seat = _mapper.Map<Seat>(seatDto);
                var updated = await _seatRepository.UpdateAsync(id, seat);
                if (updated == null) return NotFound();

         
                await RecalculateAvailableSeats(updated.ScheduleId);

                return Ok(_mapper.Map<SeatResponseDTO>(updated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            try
            {
                var seat = await _seatRepository.GetByIdAsync(id);
                if (seat == null) return NotFound();

                var success = await _seatRepository.DeleteAsync(id);

                if (success)
                    await RecalculateAvailableSeats(seat.ScheduleId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        private async Task RecalculateAvailableSeats(int scheduleId)
        {
                var seats = await _seatRepository.GetByScheduleAsync(scheduleId);
                var schedule = seats.FirstOrDefault()?.Schedule;

                if (schedule != null)
                {
                    schedule.AvailableSeats = seats.Count(s => !s.IsBooked);
      
                    await Task.Run(() => _seatRepository.UpdateAsync(seats.First().SeatId, seats.First()));
                }
        }
    }
}

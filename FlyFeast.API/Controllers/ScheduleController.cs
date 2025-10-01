using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;

        public ScheduleController(IScheduleRepository scheduleRepository, IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                var schedules = await _scheduleRepository.GetAllAsync();
                return Ok(_mapper.Map<List<ScheduleResponseDTO>>(schedules));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(id);
                if (schedule == null) return NotFound(new { error = $"Schedule with ID {id} not found." });

                return Ok(_mapper.Map<ScheduleResponseDTO>(schedule));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequestDTO scheduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var schedule = _mapper.Map<Schedule>(scheduleDto);
                var created = await _scheduleRepository.AddAsync(schedule);
                return CreatedAtAction(nameof(GetSchedule), new { id = created.ScheduleId }, _mapper.Map<ScheduleResponseDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not create schedule: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleRequestDTO scheduleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var schedule = _mapper.Map<Schedule>(scheduleDto);
                var updated = await _scheduleRepository.UpdateAsync(id, schedule);

                if (updated == null) return NotFound(new { error = $"Schedule with ID {id} not found." });

                return Ok(_mapper.Map<ScheduleResponseDTO>(updated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not update schedule: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var success = await _scheduleRepository.DeleteAsync(id);
                if (!success) return NotFound(new { error = $"Schedule with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not delete schedule: {ex.Message}" });
            }
        }
    }
}

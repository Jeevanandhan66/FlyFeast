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
    [Authorize(Roles = "Customer,Admin")]
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerRepository _passengerRepository;
        private readonly IMapper _mapper;

        public PassengerController(IPassengerRepository passengerRepository, IMapper mapper)
        {
            _passengerRepository = passengerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPassengers()
        {
            try
            {
                var passengers = await _passengerRepository.GetAllAsync();
                return Ok(_mapper.Map<List<PassengerDTO>>(passengers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPassenger(int id)
        {
            try
            {
                var passenger = await _passengerRepository.GetByIdAsync(id);
                if (passenger == null) return NotFound();

                return Ok(_mapper.Map<PassengerDTO>(passenger));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("byUser/{userId}")]
        public async Task<IActionResult> GetPassengersByUser(string userId)
        {
            try
            {
                var passengers = await _passengerRepository.GetByUserIdAsync(userId);
                return Ok(_mapper.Map<List<PassengerDTO>>(passengers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassenger(PassengerDTO passengerDto)
        {
            try
            {
                var passenger = _mapper.Map<Passenger>(passengerDto);
                var created = await _passengerRepository.AddAsync(passenger);
                return CreatedAtAction(nameof(GetPassenger), new { id = created.PassengerId }, _mapper.Map<PassengerDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassenger(int id, PassengerDTO passengerDto)
        {
            try
            {
                var passenger = _mapper.Map<Passenger>(passengerDto);
                var updated = await _passengerRepository.UpdateAsync(id, passenger);
                if (updated == null) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            try
            {
                var success = await _passengerRepository.DeleteAsync(id);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

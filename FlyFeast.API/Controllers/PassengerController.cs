using AutoMapper;
using FlyFeast.API.DTOs.User_Role;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
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
            var passengers = await _passengerRepository.GetAllAsync();
            var dtos = _mapper.Map<List<PassengerDTO>>(passengers);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPassenger(int id)
        {
            var passenger = await _passengerRepository.GetByIdAsync(id);
            if (passenger == null) return NotFound();

            var dto = _mapper.Map<PassengerDTO>(passenger);
            return Ok(dto);
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPassengerByUserId(string userId)
        {
            var passenger = await _passengerRepository.GetPassengerByUserId(userId);
            if (passenger == null)
                return NotFound(new { message = "Passenger not found for this user." });

            var dto = _mapper.Map<PassengerDTO>(passenger);
            return Ok(dto);
        }



        [HttpPost]
        public async Task<IActionResult> CreatePassenger([FromBody] PassengerRequestDTO passengerDto)
        {
            var passenger = _mapper.Map<Passenger>(passengerDto);
            var created = await _passengerRepository.AddAsync(passenger);

            var responseDto = _mapper.Map<PassengerDTO>(created);
            return CreatedAtAction(nameof(GetPassenger), new { id = created.PassengerId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassenger(int id, [FromBody] PassengerRequestDTO passengerDto)
        {
            var passenger = _mapper.Map<Passenger>(passengerDto);
            var updated = await _passengerRepository.UpdateAsync(id, passenger);
            if (updated == null) return NotFound();

            var dto = _mapper.Map<PassengerDTO>(updated);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            var success = await _passengerRepository.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}

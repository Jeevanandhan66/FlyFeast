using AutoMapper;
using FlyFeast.API.DTOs.Aircraft_Airport;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class AirportController : ControllerBase
    {
        private readonly IAirportRepository _airportRepository;
        private readonly IMapper _mapper;

        public AirportController(IAirportRepository airportRepository, IMapper mapper)
        {
            _airportRepository = airportRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAirports([FromQuery] string? search)
        {
            try
            {
                var airports = await _airportRepository.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var lower = search.ToLower();
                    airports = airports.Where(a =>
                        a.City.ToLower().Contains(lower) ||
                        a.Code.ToLower().Contains(lower) ||
                        a.AirportName.ToLower().Contains(lower)
                    ).ToList();
                }

                var dtos = _mapper.Map<List<AirportDTO>>(airports);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAirport(int id)
        {
            try
            {
                var airport = await _airportRepository.GetByIdAsync(id);
                if (airport == null) return NotFound();

                var dto = _mapper.Map<AirportDTO>(airport);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAirport([FromBody] AirportDTO airportDto)
        {
            try
            {
                var airport = _mapper.Map<Airport>(airportDto);
                var created = await _airportRepository.AddAsync(airport);

                var responseDto = _mapper.Map<AirportDTO>(created);
                return CreatedAtAction(nameof(GetAirport), new { id = created.AirportId }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAirport(int id, [FromBody] AirportDTO airportDto)
        {
            try
            {
                var airport = _mapper.Map<Airport>(airportDto);
                var updated = await _airportRepository.UpdateAsync(id, airport);
                if (updated == null) return NotFound();

                var dto = _mapper.Map<AirportDTO>(updated);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirport(int id)
        {
            try
            {
                var success = await _airportRepository.DeleteAsync(id);
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

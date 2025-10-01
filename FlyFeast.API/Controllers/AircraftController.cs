using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.DTOs.Aircraft_Airport;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AircraftController : ControllerBase
    {
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IMapper _mapper;

        public AircraftController(IAircraftRepository aircraftRepository, IMapper mapper)
        {
            _aircraftRepository = aircraftRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAircrafts()
        {
            try
            {
                var aircrafts = await _aircraftRepository.GetAllAsync();
                var dtos = _mapper.Map<List<AircraftResponseDTO>>(aircrafts);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAircraft(int id)
        {
            try
            {
                var aircraft = await _aircraftRepository.GetByIdAsync(id);
                if (aircraft == null) return NotFound();

                var dto = _mapper.Map<AircraftResponseDTO>(aircraft);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateAircraft([FromBody] AircraftRequestDTO aircraftRequest)
        {
            try
            {
                var aircraft = _mapper.Map<Aircraft>(aircraftRequest);
                var created = await _aircraftRepository.AddAsync(aircraft);


                var createdWithOwner = await _aircraftRepository.GetByIdAsync(created.AircraftId);
                var responseDto = _mapper.Map<AircraftResponseDTO>(createdWithOwner);

                return CreatedAtAction(nameof(GetAircraft), new { id = created.AircraftId }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAircraft(int id, [FromBody] AircraftRequestDTO updatedAircraftRequest)
        {
            try
            {
                var existingAircraft = await _aircraftRepository.GetByIdAsync(id);
                if (existingAircraft == null) return NotFound();

               
                _mapper.Map(updatedAircraftRequest, existingAircraft);
                var updated = await _aircraftRepository.UpdateAsync(id, existingAircraft);

                if (updated == null) return NotFound();

              
                var dto = _mapper.Map<AircraftResponseDTO>(updated);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAircraft(int id)
        {
            try
            {
                var success = await _aircraftRepository.DeleteAsync(id);
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

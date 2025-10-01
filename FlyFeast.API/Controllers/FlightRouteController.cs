using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.DTOs.Routes;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class FlightRouteController : ControllerBase
    {
        private readonly IFlightRouteRepository _routeRepository;
        private readonly IMapper _mapper;

        public FlightRouteController(IFlightRouteRepository routeRepository, IMapper mapper)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            try
            {
                var routes = await _routeRepository.GetAllAsync();
                return Ok(_mapper.Map<List<RouteResponseDTO>>(routes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoute(int id)
        {
            try
            {
                var route = await _routeRepository.GetByIdAsync(id);
                if (route == null) return NotFound();
                return Ok(_mapper.Map<RouteResponseDTO>(route));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute(RouteRequestDTO routeDto)
        {
            try
            {
                var route = _mapper.Map<FlightRoute>(routeDto);
                var created = await _routeRepository.AddAsync(route);
                return CreatedAtAction(nameof(GetRoute), new { id = created.RouteId },
                                       _mapper.Map<RouteResponseDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(int id, RouteRequestDTO routeDto)
        {
            try
            {
                var route = _mapper.Map<FlightRoute>(routeDto);
                var updated = await _routeRepository.UpdateAsync(id, route);
                if (updated == null) return NotFound();
                return Ok(_mapper.Map<RouteResponseDTO>(updated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                var success = await _routeRepository.DeleteAsync(id);
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

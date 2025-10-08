using AutoMapper;
using FlyFeast.API.DTOs.Schedules;
using FlyFeast.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IFlightSearchService _flightSearchService;
        private readonly IMapper _mapper;

        public SearchController(IFlightSearchService flightSearchService, IMapper mapper)
        {
            _flightSearchService = flightSearchService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string originCity,
            [FromQuery] string destinationCity,
            [FromQuery] DateTime date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(originCity) || string.IsNullOrWhiteSpace(destinationCity))
                    return BadRequest(new { message = "Origin and Destination are required." });

                var schedules = await _flightSearchService.SearchFlightsAsync(originCity, destinationCity, date);

                if (!schedules.Any())
                    return NotFound(new { message = "No flights available for the selected route and date." });

                var dto = _mapper.Map<List<ScheduleResponseDTO>>(schedules);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }
    }
}

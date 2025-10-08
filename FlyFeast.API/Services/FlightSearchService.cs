using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Services
{
    public class FlightSearchService : IFlightSearchService
    {
        private readonly ApplicationDbContext _context;

        public FlightSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule>> SearchFlightsAsync(string originCity, string destinationCity, DateTime date)
        {
            originCity = originCity?.Trim().ToLower() ?? "";
            destinationCity = destinationCity?.Trim().ToLower() ?? "";

            return await _context.Schedules
                .Include(s => s.Route)
                    .ThenInclude(r => r.OriginAirport)
                .Include(s => s.Route)
                    .ThenInclude(r => r.DestinationAirport)
                .Include(s => s.Route)
                    .ThenInclude(r => r.Aircraft)
                    .ThenInclude(a => a.Owner)
                .Include(s => s.Seats)
                .Where(s =>
                    (
                        s.Route.OriginAirport.City.ToLower().Contains(originCity) ||
                        s.Route.OriginAirport.Code.ToLower().Contains(originCity)
                    )
                    &&
                    (
                        s.Route.DestinationAirport.City.ToLower().Contains(destinationCity) ||
                        s.Route.DestinationAirport.Code.ToLower().Contains(destinationCity)
                    )
                    &&
                    s.DepartureTime.Date == date.Date
                )
                .OrderBy(s => s.DepartureTime)
                .ToListAsync();
        }
    }
}

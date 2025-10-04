using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class FlightSearchService : IFlightSearchService
{
    private readonly ApplicationDbContext _context;

    public FlightSearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Schedule>> SearchFlightsAsync(string originCity, string destinationCity, DateTime date)
    {
        return await _context.Schedules
            .Include(s => s.Route).ThenInclude(r => r.OriginAirport)
            .Include(s => s.Route).ThenInclude(r => r.DestinationAirport)
            .Include(s => s.Route).ThenInclude(r => r.Aircraft).ThenInclude(a => a.Owner)
            .Include(s => s.Seats)
            .Where(s =>
                s.Route.OriginAirport.City.ToLower() == originCity.ToLower() &&
                s.Route.DestinationAirport.City.ToLower() == destinationCity.ToLower() &&
                s.DepartureTime.Date == date.Date)
            .ToListAsync();
    }
}

using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class FlightRouteRepository : IFlightRouteRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightRouteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlightRoute>> GetAllAsync()
        {
            return await _context.Routes
                .Include(r => r.Aircraft)
                    .ThenInclude(a => a.Owner)
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .ToListAsync();
        }

        public async Task<FlightRoute?> GetByIdAsync(int id)
        {
            return await _context.Routes
                .Include(r => r.Aircraft)
                    .ThenInclude(a => a.Owner)
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .FirstOrDefaultAsync(r => r.RouteId == id);
        }

        public async Task<FlightRoute> AddAsync(FlightRoute route)
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            await _context.Entry(route).Reference(r => r.Aircraft).LoadAsync();
            await _context.Entry(route).Reference(r => r.OriginAirport).LoadAsync();
            await _context.Entry(route).Reference(r => r.DestinationAirport).LoadAsync();

            if (route.Aircraft != null)
                await _context.Entry(route.Aircraft).Reference(a => a.Owner).LoadAsync();

            return route;
        }

        public async Task<FlightRoute?> UpdateAsync(int id, FlightRoute route)
        {
            var existing = await _context.Routes.FindAsync(id);
            if (existing == null) return null;

            existing.AircraftId = route.AircraftId;
            existing.OriginAirportId = route.OriginAirportId;
            existing.DestinationAirportId = route.DestinationAirportId;
            existing.BaseFare = route.BaseFare;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Routes.FindAsync(id);
            if (existing == null) return false;

            _context.Routes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

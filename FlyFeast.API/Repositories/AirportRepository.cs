using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _context;

        public AirportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Airport>> GetAllAsync()
        {
            return await _context.Airports.ToListAsync();
        }

        public async Task<Airport?> GetByIdAsync(int id)
        {
            return await _context.Airports.FindAsync(id);
        }


        public async Task<Airport> AddAsync(Airport airport)
        {
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();
            return airport;
        }

        public async Task<Airport?> UpdateAsync(int id, Airport airport)
        {
            var existing = await _context.Airports.FindAsync(id);
            if (existing == null) return null;

            existing.AirportName = airport.AirportName;
            existing.Code = airport.Code;
            existing.City = airport.City;
            existing.Country = airport.Country;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Airports.FindAsync(id);
            if (existing == null) return false;

            _context.Airports.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

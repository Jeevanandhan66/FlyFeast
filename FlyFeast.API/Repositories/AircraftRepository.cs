using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class AircraftRepository : IAircraftRepository
    {
        private readonly ApplicationDbContext _context;

        public AircraftRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<Aircraft>> GetAllAsync()
        {
            return await _context.Aircrafts
                .Include(a => a.Owner)
                .ToListAsync();
        }

        public async Task<Aircraft?> GetByIdAsync(int id)
        {
            return await _context.Aircrafts
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.AircraftId == id);
        }

        public async Task<Aircraft> AddAsync(Aircraft aircraft)
        {
            aircraft.CreatedAt = DateTime.UtcNow;
            _context.Aircrafts.Add(aircraft);
            await _context.SaveChangesAsync();
            return aircraft;
        }

        public async Task<Aircraft?> UpdateAsync(int id, Aircraft aircraft)
        {
            var existing = await _context.Aircrafts.FindAsync(id);
            if (existing == null) return null;

            existing.AircraftCode = aircraft.AircraftCode;
            existing.AircraftName = aircraft.AircraftName;
            existing.OwnerId = aircraft.OwnerId;
            existing.EconomySeats = aircraft.EconomySeats;
            existing.BusinessSeats = aircraft.BusinessSeats;
            existing.FirstClassSeats = aircraft.FirstClassSeats;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Aircrafts.FindAsync(id);
            if (existing == null) return false;

            _context.Aircrafts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

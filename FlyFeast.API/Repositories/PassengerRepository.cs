using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly ApplicationDbContext _context;

        public PassengerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Passenger>> GetAllAsync()
        {
            return await _context.Passengers
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Passenger?> GetByIdAsync(int id)
        {
            return await _context.Passengers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PassengerId == id);
        }

        public async Task<Passenger?> GetPassengerByUserId(string userId)
        {
            return await _context.Passengers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }


        public async Task<Passenger> AddAsync(Passenger passenger)
        {
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();
            return passenger;
        }

        public async Task<Passenger?> UpdateAsync(int id, Passenger passenger)
        {
            var existing = await _context.Passengers.FindAsync(id);
            if (existing == null) return null;

            existing.UserId = passenger.UserId;
            existing.DateOfBirth = passenger.DateOfBirth;
            existing.PassportNumber = passenger.PassportNumber;
            existing.Nationality = passenger.Nationality;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Passengers.FindAsync(id);
            if (existing == null) return false;

            _context.Passengers.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

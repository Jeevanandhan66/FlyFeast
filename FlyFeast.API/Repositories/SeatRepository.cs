using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Seat>> GetAllAsync()
        {
            return await _context.Seats
                .Include(s => s.Schedule)
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .Include(s => s.Schedule)
                .FirstOrDefaultAsync(s => s.SeatId == id);
        }

        public async Task<List<Seat>> GetByScheduleAsync(int scheduleId)
        {
            return await _context.Seats
                .Where(s => s.ScheduleId == scheduleId)
                .Include(s => s.Schedule)
                .ToListAsync();
        }

        public async Task<Seat> AddAsync(Seat seat)
        {
            bool exists = await _context.Seats
                .AnyAsync(s => s.ScheduleId == seat.ScheduleId && s.SeatNumber == seat.SeatNumber);

            if (exists)
                throw new InvalidOperationException($"Seat {seat.SeatNumber} already exists in this schedule.");

            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            // recalc available seats
            await UpdateAvailableSeats(seat.ScheduleId);

            return seat;
        }

        public async Task<Seat?> UpdateAsync(int id, Seat seat)
        {
            var existing = await _context.Seats.FindAsync(id);
            if (existing == null) return null;

            // prevent duplicate seat number
            bool exists = await _context.Seats
                .AnyAsync(s => s.ScheduleId == seat.ScheduleId && s.SeatNumber == seat.SeatNumber && s.SeatId != id);
            if (exists)
                throw new InvalidOperationException($"Seat {seat.SeatNumber} already exists in this schedule.");

            existing.SeatNumber = seat.SeatNumber;
            existing.Class = seat.Class;
            existing.Price = seat.Price;
            existing.IsBooked = seat.IsBooked;
            existing.ScheduleId = seat.ScheduleId;

            await _context.SaveChangesAsync();

            await UpdateAvailableSeats(existing.ScheduleId);

            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Seats.FindAsync(id);
            if (existing == null) return false;

            int scheduleId = existing.ScheduleId;

            _context.Seats.Remove(existing);
            await _context.SaveChangesAsync();

            await UpdateAvailableSeats(scheduleId);

            return true;
        }

        private async Task UpdateAvailableSeats(int scheduleId)
        {
            var schedule = await _context.Schedules.FindAsync(scheduleId);
            if (schedule != null)
            {
                schedule.AvailableSeats = await _context.Seats
                    .CountAsync(s => s.ScheduleId == scheduleId && !s.IsBooked);

                await _context.SaveChangesAsync();
            }
        }
    }
}

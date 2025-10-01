using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule>> GetAllAsync()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Route)
                    .ThenInclude(r => r.Aircraft)
                        .ThenInclude(a => a.Owner)
                .Include(s => s.Route.OriginAirport)
                .Include(s => s.Route.DestinationAirport)
                .Include(s => s.Seats)
                .ToListAsync();

            foreach (var schedule in schedules)
            {
                schedule.AvailableSeats = schedule.Seats?.Count(seat => !seat.IsBooked) ?? 0;
            }

            return schedules;
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Route)
                    .ThenInclude(r => r.Aircraft)
                        .ThenInclude(a => a.Owner)
                .Include(s => s.Route.OriginAirport)
                .Include(s => s.Route.DestinationAirport)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule != null)
                schedule.AvailableSeats = schedule.Seats?.Count(seat => !seat.IsBooked) ?? 0;

            return schedule;
        }

        public async Task<Schedule> AddAsync(Schedule schedule)
        {

            schedule.AvailableSeats = schedule.SeatCapacity;
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            var route = await _context.Routes
                .Include(r => r.Aircraft)
                .FirstOrDefaultAsync(r => r.RouteId == schedule.RouteId);

            if (route?.Aircraft != null)
            {
                var aircraft = route.Aircraft;
                var seats = new List<Seat>();
                int seatCounter = 1;

                // Economy
                for (int i = 0; i < aircraft.EconomySeats; i++)
                    seats.Add(CreateSeat(schedule.ScheduleId, $"E{seatCounter++}", "Economy", route.BaseFare));

                // Business
                for (int i = 0; i < aircraft.BusinessSeats; i++)
                    seats.Add(CreateSeat(schedule.ScheduleId, $"B{seatCounter++}", "Business", route.BaseFare * 1.5m));

                // First
                for (int i = 0; i < aircraft.FirstClassSeats; i++)
                    seats.Add(CreateSeat(schedule.ScheduleId, $"F{seatCounter++}", "First", route.BaseFare * 2m));

                _context.Seats.AddRange(seats);
                await _context.SaveChangesAsync();
            }

            return await GetByIdAsync(schedule.ScheduleId) ?? schedule;
        }

        public async Task<Schedule?> UpdateAsync(int id, Schedule schedule)
        {
            var existing = await _context.Schedules.FindAsync(id);
            if (existing == null) return null;

            existing.RouteId = schedule.RouteId;
            existing.DepartureTime = schedule.DepartureTime;
            existing.ArrivalTime = schedule.ArrivalTime;
            existing.SeatCapacity = schedule.SeatCapacity;
            existing.Status = schedule.Status;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Schedules.FindAsync(id);
            if (existing == null) return false;

            _context.Schedules.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
        private Seat CreateSeat(int scheduleId, string seatNumber, string seatClass, decimal price)
        {
            return new Seat
            {
                ScheduleId = scheduleId,
                SeatNumber = seatNumber,
                Class = seatClass,
                Price = price,
                IsBooked = false
            };
        }
    }
}

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

            // recalc available seats
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
            // Validate time range
            if (schedule.ArrivalTime <= schedule.DepartureTime)
                throw new InvalidOperationException("Arrival time must be later than departure time.");

            // Load route + aircraft
            var route = await _context.Routes
                .Include(r => r.Aircraft)
                .FirstOrDefaultAsync(r => r.RouteId == schedule.RouteId);

            if (route?.Aircraft == null)
                throw new InvalidOperationException("Invalid Route or Aircraft not found.");

            var aircraft = route.Aircraft;

            // Auto-set seat capacity from aircraft
            schedule.SeatCapacity = aircraft.EconomySeats + aircraft.BusinessSeats + aircraft.FirstClassSeats;
            schedule.AvailableSeats = schedule.SeatCapacity;

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Generate seats
            var seats = new List<Seat>();
            int seatCounter = 1;

            // Economy
            // Economy
            for (int i = 0; i < aircraft.EconomySeats; i++)
                seats.Add(CreateSeat(schedule.ScheduleId, $"E{seatCounter++}", SeatClass.Economy, route.BaseFare));

            // Business
            for (int i = 0; i < aircraft.BusinessSeats; i++)
                seats.Add(CreateSeat(schedule.ScheduleId, $"B{seatCounter++}", SeatClass.Business, route.BaseFare * 1.5m));

            // First
            for (int i = 0; i < aircraft.FirstClassSeats; i++)
                seats.Add(CreateSeat(schedule.ScheduleId, $"F{seatCounter++}", SeatClass.First, route.BaseFare * 2m));


            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(schedule.ScheduleId) ?? schedule;
        }

        public async Task<Schedule?> UpdateAsync(int id, Schedule schedule)
        {
            var existing = await _context.Schedules.FindAsync(id);
            if (existing == null) return null;

            if (schedule.ArrivalTime <= schedule.DepartureTime)
                throw new InvalidOperationException("Arrival time must be later than departure time.");

            existing.RouteId = schedule.RouteId;
            existing.DepartureTime = schedule.DepartureTime;
            existing.ArrivalTime = schedule.ArrivalTime;
            existing.Status = schedule.Status;

            // SeatCapacity stays tied to the aircraft, don’t overwrite manually
            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null)
                return false;

            if (schedule.Bookings != null && schedule.Bookings.Any())
                throw new InvalidOperationException("Cannot delete schedule with active bookings.");

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }


        private Seat CreateSeat(int scheduleId, string seatNumber, SeatClass seatClass, decimal price)
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

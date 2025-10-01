using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Schedule)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Seat)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Passenger)
                .Include(b => b.Payments)
                .Include(b => b.Refunds)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Schedule)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Seat)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Passenger)
                .Include(b => b.Payments)
                .Include(b => b.Refunds)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<List<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Schedule)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Seat)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Passenger)
                .Include(b => b.Payments)
                .Include(b => b.Refunds)
                .ToListAsync();
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            booking.BookingRef = $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
            booking.CreatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> AddAsync(Booking booking, List<(int SeatId, int PassengerId)> seatPassengerPairs)
        {
            booking.BookingRef = $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
            booking.CreatedAt = DateTime.UtcNow;

            var schedule = await _context.Schedules.FindAsync(booking.ScheduleId);
            if (schedule == null) throw new Exception("Schedule not found");

            decimal totalAmount = 0;
            var bookingItems = new List<BookingItem>();

            foreach (var (seatId, passengerId) in seatPassengerPairs)
            {
                var seat = await _context.Seats
                    .FirstOrDefaultAsync(s => s.SeatId == seatId && s.ScheduleId == booking.ScheduleId);
                if (seat == null) throw new Exception($"Seat {seatId} not found for schedule {booking.ScheduleId}");
                if (seat.IsBooked) throw new Exception($"Seat {seat.SeatNumber} is already booked.");

                seat.IsBooked = true;

                bookingItems.Add(new BookingItem
                {
                    SeatId = seat.SeatId,
                    PassengerId = passengerId,
                    PriceAtBooking = seat.Price
                });

                totalAmount += seat.Price;
            }

            booking.BookingItems = bookingItems;
            booking.TotalAmount = totalAmount;
            booking.Status = "Confirmed";

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<Booking?> UpdateAsync(int id, Booking booking)
        {
            var existing = await _context.Bookings.FindAsync(id);
            if (existing == null) return null;

            existing.Status = booking.Status;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Bookings.FindAsync(id);
            if (existing == null) return false;

            _context.Bookings.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddCancellationAsync(BookingCancellation cancellation)
        {
            _context.BookingCancellations.Add(cancellation);
            await _context.SaveChangesAsync();
        }

        public async Task ReleaseSeatsAsync(int bookingId)
        {
            var bookingItems = await _context.BookingItems
                .Include(bi => bi.Seat)
                .Where(bi => bi.BookingId == bookingId)
                .ToListAsync();

            foreach (var item in bookingItems)
            {
                if (item.Seat != null)
                {
                    item.Seat.IsBooked = false;
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}

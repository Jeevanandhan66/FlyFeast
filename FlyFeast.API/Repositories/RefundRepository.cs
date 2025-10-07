using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Repositories
{
    public class RefundRepository : IRefundRepository
    {
        private readonly ApplicationDbContext _context;

        public RefundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Refund>> GetAllAsync()
        {
            return await _context.Refunds
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Schedule)
                .Include(r => r.ProcessedUser)
                .ToListAsync();
        }

        public async Task<Refund?> GetByIdAsync(int id)
        {
            return await _context.Refunds
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Schedule)
                .Include(r => r.ProcessedUser)
                .FirstOrDefaultAsync(r => r.RefundId == id);
        }

        public async Task<List<Refund>> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Refunds
                .Where(r => r.BookingId == bookingId)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Schedule)
                .Include(r => r.ProcessedUser)
                .ToListAsync();
        }

        public async Task<Refund> AddAsync(Refund refund)
        {
            refund.CreatedAt = DateTime.UtcNow;
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(refund.RefundId)
                ?? throw new Exception("Refund not found after insert");
        }

        public async Task<Refund?> UpdateAsync(int id, Refund refund)
        {
            var existing = await _context.Refunds.FindAsync(id);
            if (existing == null) return null;

            existing.BookingId = refund.BookingId;
            existing.Amount = refund.Amount;
            existing.Status = refund.Status;
            existing.ProcessedById = refund.ProcessedById;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Refunds.FindAsync(id);
            if (existing == null) return false;

            _context.Refunds.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

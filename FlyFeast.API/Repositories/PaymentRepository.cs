using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<List<Payment>> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                .ToListAsync();
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            payment.CreatedAt = DateTime.UtcNow;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                .FirstOrDefaultAsync(p => p.PaymentId == payment.PaymentId)
                ?? throw new Exception("Payment not found after insert");
        }

        public async Task<Payment?> UpdateAsync(int id, Payment payment)
        {
            var existing = await _context.Payments.FindAsync(id);
            if (existing == null) return null;

            existing.BookingId = payment.BookingId;
            existing.Amount = payment.Amount;
            existing.Provider = payment.Provider;
            existing.Status = payment.Status;

            if (!string.IsNullOrEmpty(payment.ProviderRef))
                existing.ProviderRef = payment.ProviderRef;

            await _context.SaveChangesAsync();
            return await _context.Payments
                .Include(p => p.Booking).ThenInclude(b => b.User)
                .Include(p => p.Booking).ThenInclude(b => b.Schedule)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }




        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Payments.FindAsync(id);
            if (existing == null) return false;

            _context.Payments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

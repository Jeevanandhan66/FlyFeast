using FlyFeast.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<List<Payment>> GetByBookingIdAsync(int bookingId);
        Task<Payment> AddAsync(Payment payment);
        Task<Payment?> UpdateAsync(int id, Payment payment);
        Task<bool> DeleteAsync(int id);
    }
}

using FlyFeast.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IRefundRepository
    {
        Task<List<Refund>> GetAllAsync();
        Task<Refund?> GetByIdAsync(int id);
        Task<List<Refund>> GetByBookingIdAsync(int bookingId);
        Task<Refund> AddAsync(Refund refund);
        Task<Refund?> UpdateAsync(int id, Refund refund);
        Task<bool> DeleteAsync(int id);
    }
}

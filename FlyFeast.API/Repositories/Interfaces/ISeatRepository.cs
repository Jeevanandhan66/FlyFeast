using FlyFeast.API.Models;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetAllAsync();
        Task<Seat?> GetByIdAsync(int id);
        Task<List<Seat>> GetByScheduleAsync(int scheduleId);
        Task<Seat> AddAsync(Seat seat);
        Task<Seat?> UpdateAsync(int id, Seat seat);
        Task<bool> DeleteAsync(int id);
    }
}

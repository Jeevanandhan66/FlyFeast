using FlyFeast.API.Models;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IAircraftRepository
    {
        Task<List<Aircraft>> GetAllAsync();
        Task<Aircraft?> GetByIdAsync(int id);
        Task<Aircraft> AddAsync(Aircraft aircraft);
        Task<Aircraft?> UpdateAsync(int id, Aircraft aircraft);
        Task<bool> DeleteAsync(int id);
    }
}

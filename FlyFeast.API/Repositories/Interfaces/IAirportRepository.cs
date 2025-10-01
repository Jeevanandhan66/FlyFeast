using FlyFeast.API.Models;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IAirportRepository
    {
        Task<List<Airport>> GetAllAsync();
        Task<Airport?> GetByIdAsync(int id);
        Task<Airport> AddAsync(Airport airport);
        Task<Airport?> UpdateAsync(int id, Airport airport);
        Task<bool> DeleteAsync(int id);
    }
}

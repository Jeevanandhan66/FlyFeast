using FlyFeast.API.Models;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IFlightRouteRepository
    {
        Task<List<FlightRoute>> GetAllAsync();
        Task<FlightRoute?> GetByIdAsync(int id);
        Task<FlightRoute> AddAsync(FlightRoute route);
        Task<FlightRoute?> UpdateAsync(int id, FlightRoute route);
        Task<bool> DeleteAsync(int id);
    }
}

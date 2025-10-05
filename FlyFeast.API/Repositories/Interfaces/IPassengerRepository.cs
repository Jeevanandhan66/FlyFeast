using FlyFeast.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IPassengerRepository
    {
        Task<List<Passenger>> GetAllAsync();
        Task<Passenger?> GetByIdAsync(int id);
        Task<Passenger?> GetPassengerByUserId(string userId);


        Task<Passenger> AddAsync(Passenger passenger);
        Task<Passenger?> UpdateAsync(int id, Passenger passenger);
        Task<bool> DeleteAsync(int id);
    }
}

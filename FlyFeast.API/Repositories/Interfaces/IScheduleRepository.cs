using FlyFeast.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task<Schedule> AddAsync(Schedule schedule);
        Task<Schedule?> UpdateAsync(int id, Schedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}

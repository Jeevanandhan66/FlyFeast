using FlyFeast.API.Models;

namespace FlyFeast.API.Services.Interfaces
{
    public interface IFlightSearchService
    {
        Task<List<Schedule>> SearchFlightsAsync(string originCity, string destinationCity, DateTime date);
    }
}

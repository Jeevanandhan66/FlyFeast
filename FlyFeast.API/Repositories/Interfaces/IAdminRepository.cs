using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;

namespace FlyFeast.API.Data.Repositories
{
    public interface IAdminRepository
    {
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password, IEnumerable<string> roles);
        Task<IdentityResult> AssignRoleAsync(string userId, string roleName);


        Task<List<IdentityRole>> GetRolesAsync();
        Task<IdentityRole> AddRoleAsync(string roleName);

        Task<List<Aircraft>> GetAircraftsAsync();
        Task<Aircraft> AddAircraftAsync(Aircraft aircraft);

        Task<List<Airport>> GetAirportsAsync();
        Task<Airport> AddAirportAsync(Airport airport);

        Task<List<FlightRoute>> GetRoutesAsync();
        Task<FlightRoute> AddRouteAsync(FlightRoute route);


        Task<List<Schedule>> GetSchedulesAsync();
        Task<Schedule> AddScheduleAsync(Schedule schedule);


        Task<List<Seat>> GetSeatsAsync();
        Task<Seat> AddSeatAsync(Seat seat);

  
        Task<List<Booking>> GetBookingsAsync();


        Task<List<Payment>> GetPaymentsAsync();
        Task<Payment> AddPaymentAsync(Payment payment);

        Task<List<Refund>> GetRefundsAsync();
        Task<Refund> AddRefundAsync(Refund refund);
    }
}

using FlyFeast.API.Data;
using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlyFeast.API.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _userManager.Users
                .Include(u => u.Passenger)
                .ToListAsync();
        }

        public async Task<ApplicationUser> AddUserAsync(ApplicationUser user, string password, IEnumerable<string> roles)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);
            }

            return user;
        }

        public async Task<IdentityResult> AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole> AddRoleAsync(string roleName)
        {
            var role = new IdentityRole(roleName);
            await _roleManager.CreateAsync(role);
            return role;
        }

        public async Task<List<Aircraft>> GetAircraftsAsync()
        {
            return await _context.Aircrafts.Include(a => a.Owner).ToListAsync();
        }

        public async Task<Aircraft> AddAircraftAsync(Aircraft aircraft)
        {
            _context.Aircrafts.Add(aircraft);
            await _context.SaveChangesAsync();
            return aircraft;
        }


        public async Task<List<Airport>> GetAirportsAsync()
        {
            return await _context.Airports.ToListAsync();
        }

        public async Task<Airport> AddAirportAsync(Airport airport)
        {
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();
            return airport;
        }


        public async Task<List<FlightRoute>> GetRoutesAsync()
        {
            return await _context.Routes
                .Include(r => r.Aircraft).ThenInclude(a => a.Owner)
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .ToListAsync();
        }

        public async Task<FlightRoute> AddRouteAsync(FlightRoute route)
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
            return route;
        }


        public async Task<List<Schedule>> GetSchedulesAsync()
        {
            return await _context.Schedules
                .Include(s => s.Route)
                    .ThenInclude(r => r.Aircraft)
                        .ThenInclude(a => a.Owner)
                .Include(s => s.Route.OriginAirport)
                .Include(s => s.Route.DestinationAirport)
                .Include(s => s.Seats)
                .ToListAsync();
        }

        public async Task<Schedule> AddScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }


        public async Task<List<Seat>> GetSeatsAsync()
        {
            return await _context.Seats.Include(s => s.Schedule).ToListAsync();
        }

        public async Task<Seat> AddSeatAsync(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();
            return seat;
        }


        public async Task<List<Booking>> GetBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Schedule)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Seat)
                .Include(b => b.BookingItems).ThenInclude(bi => bi.Passenger)
                .Include(b => b.Payments)
                .Include(b => b.Refunds)
                .ToListAsync();
        }


        public async Task<List<Payment>> GetPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                .ToListAsync();
        }

        public async Task<Payment> AddPaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }


        public async Task<List<Refund>> GetRefundsAsync()
        {
            return await _context.Refunds
                .Include(r => r.Booking)
                    .ThenInclude(b => b.User)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Schedule)
                .Include(r => r.ProcessedUser)
                .ToListAsync();
        }

        public async Task<Refund> AddRefundAsync(Refund refund)
        {
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();
            return refund;
        }
    }
}

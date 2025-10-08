using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class ScheduleRepositoryTests
    {
        private ApplicationDbContext _context;
        private ScheduleRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"ScheduleDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new ScheduleRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private FlightRoute SeedRouteWithAircraft()
        {
            var aircraft = new Aircraft
            {
                AircraftCode = "AC1",
                AircraftName = "PlaneX",
                EconomySeats = 2,
                BusinessSeats = 1,
                FirstClassSeats = 1,
                CreatedAt = DateTime.Now
            };

            var origin = new Airport { AirportName = "Origin", Code = "ORG", City = "CityO", Country = "CountryO" };
            var destination = new Airport { AirportName = "Dest", Code = "DST", City = "CityD", Country = "CountryD" };

            var route = new FlightRoute
            {
                Aircraft = aircraft,
                OriginAirport = origin,
                DestinationAirport = destination,
                BaseFare = 100
            };

            _context.Aircrafts.Add(aircraft);
            _context.Airports.AddRange(origin, destination);
            _context.Routes.Add(route);
            _context.SaveChanges();

            return route;
        }

        [Test]
        public async Task AddAsync_ShouldCreateSchedule_AndGenerateSeats()
        {
            var route = SeedRouteWithAircraft();

            var schedule = new Schedule
            {
                RouteId = route.RouteId,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3),
                Status = ScheduleStatus.Scheduled
            };

            var added = await _repo.AddAsync(schedule);

            Assert.That(added.ScheduleId, Is.GreaterThan(0));
            Assert.That(added.Seats.Count, Is.EqualTo(4)); // 2E + 1B + 1F
            Assert.That(added.AvailableSeats, Is.EqualTo(4));
        }

        [Test]
        public void AddAsync_ShouldThrow_WhenArrivalBeforeDeparture()
        {
            var route = SeedRouteWithAircraft();

            var schedule = new Schedule
            {
                RouteId = route.RouteId,
                DepartureTime = DateTime.Now.AddHours(5),
                ArrivalTime = DateTime.Now.AddHours(1),
                Status = ScheduleStatus.Scheduled
            };

            Assert.ThrowsAsync<InvalidOperationException>(() => _repo.AddAsync(schedule));
        }



        [Test]
        public async Task DeleteAsync_ShouldRemoveSchedule()
        {
            var route = SeedRouteWithAircraft();

            var schedule = new Schedule
            {
                RouteId = route.RouteId,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(2),
                Status = ScheduleStatus.Scheduled
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteAsync(schedule.ScheduleId);

            Assert.That(result, Is.True);
            Assert.That(await _repo.GetByIdAsync(schedule.ScheduleId), Is.Null);
        }

        [Test]
        public async Task DeleteAsync_ShouldThrow_WhenHasBookings()
        {
            var route = SeedRouteWithAircraft();

            var schedule = new Schedule
            {
                RouteId = route.RouteId,
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(2),
                Status = ScheduleStatus.Scheduled,
                Bookings = new[] { new Booking { UserId = "U1", CreatedAt = DateTime.Now, Status = BookingStatus.Pending } }
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<InvalidOperationException>(() => _repo.DeleteAsync(schedule.ScheduleId));
        }
    }
}

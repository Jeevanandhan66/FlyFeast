using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class FlightRouteRepositoryTests
    {
        private ApplicationDbContext _context;
        private FlightRouteRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"FlightRouteTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new FlightRouteRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        private (Aircraft aircraft, Airport origin, Airport destination) SeedBaseData()
        {
            var owner = new ApplicationUser { Id = "owner1", UserName = "owner@test.com", Email = "owner@test.com" };
            var aircraft = new Aircraft { AircraftCode = "ACX", AircraftName = "TestPlane", Owner = owner };
            var origin = new Airport { AirportName = "Origin Airport", Code = "ORG", City = "CityO", Country = "CO" };
            var destination = new Airport { AirportName = "Dest Airport", Code = "DST", City = "CityD", Country = "CD" };

            _context.Users.Add(owner);
            _context.Aircrafts.Add(aircraft);
            _context.Airports.AddRange(origin, destination);
            _context.SaveChanges();

            return (aircraft, origin, destination);
        }

        [Test]
        public async Task AddAsync_ShouldAddRoute()
        {
            var (aircraft, origin, destination) = SeedBaseData();

            var route = new FlightRoute
            {
                AircraftId = aircraft.AircraftId,
                OriginAirportId = origin.AirportId,
                DestinationAirportId = destination.AirportId,
                BaseFare = 200
            };

            var added = await _repo.AddAsync(route);

            Assert.That(added.RouteId, Is.GreaterThan(0));
            Assert.That(added.OriginAirport.AirportName, Is.EqualTo("Origin Airport"));
            Assert.That(added.DestinationAirport.AirportName, Is.EqualTo("Dest Airport"));
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsRoute()
        {
            var (aircraft, origin, destination) = SeedBaseData();

            var route = new FlightRoute
            {
                AircraftId = aircraft.AircraftId,
                OriginAirportId = origin.AirportId,
                DestinationAirportId = destination.AirportId,
                BaseFare = 150
            };
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            var found = await _repo.GetByIdAsync(route.RouteId);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.BaseFare, Is.EqualTo(150));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateBaseFare()
        {
            var (aircraft, origin, destination) = SeedBaseData();

            var route = new FlightRoute
            {
                AircraftId = aircraft.AircraftId,
                OriginAirportId = origin.AirportId,
                DestinationAirportId = destination.AirportId,
                BaseFare = 300
            };
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            route.BaseFare = 400;
            var updated = await _repo.UpdateAsync(route.RouteId, route);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.BaseFare, Is.EqualTo(400));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveRoute()
        {
            var (aircraft, origin, destination) = SeedBaseData();

            var route = new FlightRoute
            {
                AircraftId = aircraft.AircraftId,
                OriginAirportId = origin.AirportId,
                DestinationAirportId = destination.AirportId,
                BaseFare = 250
            };
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteAsync(route.RouteId);

            Assert.That(result, Is.True);
            Assert.That(await _repo.GetByIdAsync(route.RouteId), Is.Null);
        }
    }
}

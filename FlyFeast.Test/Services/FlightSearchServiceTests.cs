using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Tests.Services
{
    [TestFixture]
    public class FlightSearchServiceTests
    {
        private ApplicationDbContext _context;
        private FlightSearchService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"FlightSearchDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new FlightSearchService(_context);

            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedData()
        {
            var aircraft = new Aircraft { AircraftCode = "AC1", AircraftName = "TestPlane", EconomySeats = 50 };
            var owner = new ApplicationUser { Id = "O1", UserName = "owner@test.com" };
            aircraft.Owner = owner;

            var origin = new Airport { AirportName = "Origin Airport", Code = "ORG", City = "OriginCity", Country = "X" };
            var destination = new Airport { AirportName = "Destination Airport", Code = "DST", City = "DestCity", Country = "Y" };

            var route = new FlightRoute
            {
                Aircraft = aircraft,
                OriginAirport = origin,
                DestinationAirport = destination,
                BaseFare = 100
            };

            var schedule1 = new Schedule
            {
                Route = route,
                DepartureTime = DateTime.Today.AddHours(10),
                ArrivalTime = DateTime.Today.AddHours(14),
                Status = ScheduleStatus.Scheduled
            };

            var schedule2 = new Schedule
            {
                Route = route,
                DepartureTime = DateTime.Today.AddHours(15),
                ArrivalTime = DateTime.Today.AddHours(19),
                Status = ScheduleStatus.Scheduled
            };

            _context.Schedules.AddRange(schedule1, schedule2);
            _context.SaveChanges();
        }

        [Test]
        public async Task SearchFlightsAsync_WhenMatch_ReturnsSchedules()
        {
            var results = await _service.SearchFlightsAsync("OriginCity", "DestCity", DateTime.Today);

            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].DepartureTime, Is.LessThan(results[1].DepartureTime));
        }

        [Test]
        public async Task SearchFlightsAsync_WhenNoMatch_ReturnsEmpty()
        {
            var results = await _service.SearchFlightsAsync("WrongCity", "Nowhere", DateTime.Today);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public async Task SearchFlightsAsync_WhenMatchByAirportCode_ReturnsSchedules()
        {
            var results = await _service.SearchFlightsAsync("ORG", "DST", DateTime.Today);

            Assert.That(results.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task SearchFlightsAsync_WhenDifferentDate_ReturnsEmpty()
        {
            var results = await _service.SearchFlightsAsync("OriginCity", "DestCity", DateTime.Today.AddDays(1));

            Assert.That(results, Is.Empty);
        }
    }
}

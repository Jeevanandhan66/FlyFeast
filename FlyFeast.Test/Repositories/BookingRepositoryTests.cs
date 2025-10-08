using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class BookingRepositoryTests
    {
        private ApplicationDbContext _context;
        private BookingRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new BookingRepository(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddBooking()
        {
            var booking = new Booking { UserId = "U1", ScheduleId = 0 };
            var created = await _repo.AddAsync(booking);

            Assert.That(created.BookingId, Is.GreaterThan(0));
            Assert.That(created.Status, Is.EqualTo(BookingStatus.Pending));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnBookings()
        {
            // Arrange: create airports
            var origin = new Airport { AirportName = "Origin", Code = "ORG", City = "CityO", Country = "CO" };
            var destination = new Airport { AirportName = "Destination", Code = "DST", City = "CityD", Country = "CD" };

            // Create aircraft
            var aircraft = new Aircraft { AircraftCode = "AC1", AircraftName = "Plane1" };

            // Create route
            var route = new FlightRoute
            {
                OriginAirport = origin,
                DestinationAirport = destination,
                Aircraft = aircraft
            };

            // Create schedule
            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                SeatCapacity = 10,
                AvailableSeats = 10,
                Status = ScheduleStatus.Scheduled,
                Route = route
            };

            // Create users
            var user1 = new ApplicationUser { Id = "U1", UserName = "u1@test.com", Email = "u1@test.com" };
            var user2 = new ApplicationUser { Id = "U2", UserName = "u2@test.com", Email = "u2@test.com" };
            _context.Users.AddRange(user1, user2);

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Act: add 2 bookings linked to schedule & users
            await _repo.AddAsync(new Booking { UserId = user1.Id, ScheduleId = schedule.ScheduleId });
            await _repo.AddAsync(new Booking { UserId = user2.Id, ScheduleId = schedule.ScheduleId });

            var result = await _repo.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }


        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsBooking()
        {
            // Arrange: seed airports
            var origin = new Airport { AirportName = "Origin", Code = "ORG", City = "CityO", Country = "CO" };
            var destination = new Airport { AirportName = "Destination", Code = "DST", City = "CityD", Country = "CD" };

            // Seed aircraft
            var aircraft = new Aircraft { AircraftCode = "ACX", AircraftName = "TestPlane" };

            // Seed route
            var route = new FlightRoute
            {
                OriginAirport = origin,
                DestinationAirport = destination,
                Aircraft = aircraft
            };

            // Seed schedule
            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                SeatCapacity = 5,
                AvailableSeats = 5,
                Status = ScheduleStatus.Scheduled,
                Route = route
            };

            // Seed user
            var user = new ApplicationUser { Id = "U9", UserName = "user9@test.com", Email = "user9@test.com" };
            _context.Users.Add(user);
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Act: create booking linked to schedule + user
            var added = await _repo.AddAsync(new Booking { UserId = user.Id, ScheduleId = schedule.ScheduleId });

            var found = await _repo.GetByIdAsync(added.BookingId);

            // Assert
            Assert.That(found, Is.Not.Null);
            Assert.That(found!.UserId, Is.EqualTo(user.Id));
            Assert.That(found.Schedule, Is.Not.Null);
            Assert.That(found.Schedule.Route.OriginAirport.AirportName, Is.EqualTo("Origin"));
        }


        [Test]
        public async Task UpdateAsync_WhenExists_ChangesStatus()
        {
            var added = await _repo.AddAsync(new Booking { UserId = "U10" });

            var updated = await _repo.UpdateAsync(added.BookingId,
                new Booking { Status = BookingStatus.Confirmed });

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Status, Is.EqualTo(BookingStatus.Confirmed));
        }

        [Test]
        public async Task UpdateAsync_WhenNotExists_ReturnsNull()
        {
            var updated = await _repo.UpdateAsync(999,
                new Booking { Status = BookingStatus.Cancelled });

            Assert.That(updated, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_RemovesBooking()
        {
            var added = await _repo.AddAsync(new Booking { UserId = "U11" });

            var result = await _repo.DeleteAsync(added.BookingId);

            Assert.That(result, Is.True);
            Assert.That(await _context.Bookings.FindAsync(added.BookingId), Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
        {
            var result = await _repo.DeleteAsync(1234);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddCancellationAsync_ShouldCancelBooking_AndReleaseSeats()
        {
            // Seed airports
            var origin = new Airport { AirportName = "Origin", Code = "ORG", City = "O", Country = "CO" };
            var destination = new Airport { AirportName = "Dest", Code = "DST", City = "D", Country = "CD" };

            // Seed aircraft
            var aircraft = new Aircraft { AircraftCode = "ACX", AircraftName = "PlaneX" };

            // Seed route
            var route = new FlightRoute
            {
                OriginAirport = origin,
                DestinationAirport = destination,
                Aircraft = aircraft
            };

            // Seed schedule with one booked seat
            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                SeatCapacity = 1,
                AvailableSeats = 0,
                Status = ScheduleStatus.Scheduled,
                Route = route,
                Seats = new List<Seat>
        {
            new Seat { SeatNumber = "1A", Class = SeatClass.Economy, Price = 100, IsBooked = true }
        }
            };

            // Seed user
            var user = new ApplicationUser { Id = "U12", UserName = "u12@test.com", Email = "u12@test.com" };

            _context.Users.Add(user);
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Create booking with booking item
            var booking = new Booking
            {
                UserId = user.Id,
                ScheduleId = schedule.ScheduleId,
                Schedule = schedule,
                Status = BookingStatus.Confirmed,
                BookingItems = new List<BookingItem>()
            };

            var seat = schedule.Seats.First();

            booking.BookingItems.Add(new BookingItem
            {
                Seat = seat,
                PassengerId = 1,
                PriceAtBooking = seat.Price,
                Booking = booking
            });

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act: cancel
            var cancellation = new BookingCancellation
            {
                BookingId = booking.BookingId,
                CancelledById = user.Id,
                Reason = "Test cancel",
                CancelledAt = DateTime.Now
            };

            await _repo.AddCancellationAsync(cancellation);

            // Assert: booking cancelled
            var cancelled = await _repo.GetByIdAsync(booking.BookingId);
            Assert.That(cancelled, Is.Not.Null);
            Assert.That(cancelled!.Status, Is.EqualTo(BookingStatus.Cancelled));

            // Assert: seat released
            var refreshedSeat = await _context.Seats.FirstAsync();
            Assert.That(refreshedSeat.IsBooked, Is.False);

            // Assert: available seats incremented
            var refreshedSchedule = await _context.Schedules.FindAsync(schedule.ScheduleId);
            Assert.That(refreshedSchedule!.AvailableSeats, Is.EqualTo(1));
        }




    }
}

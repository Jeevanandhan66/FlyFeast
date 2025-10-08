using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class RefundRepositoryTests
    {
        private ApplicationDbContext _context;
        private RefundRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"RefundDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new RefundRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private Booking SeedBooking()
        {
            var user = new ApplicationUser { Id = "U1", UserName = "cust@test.com", Email = "cust@test.com" };
            var schedule = new Schedule
            {
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                SeatCapacity = 10,
                AvailableSeats = 10,
                Status = ScheduleStatus.Scheduled
            };

            var booking = new Booking
            {
                User = user,
                UserId = user.Id,
                Schedule = schedule,
                ScheduleId = schedule.ScheduleId,
                CreatedAt = DateTime.Now,
                Status = BookingStatus.Confirmed
            };

            _context.Users.Add(user);
            _context.Schedules.Add(schedule);
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking;
        }

        private ApplicationUser SeedProcessor()
        {
            var admin = new ApplicationUser { Id = "Admin1", UserName = "admin@test.com", Email = "admin@test.com" };
            _context.Users.Add(admin);
            _context.SaveChanges();
            return admin;
        }

        [Test]
        public async Task AddAsync_ShouldAddRefund()
        {
            var booking = SeedBooking();
            var processor = SeedProcessor();

            var refund = new Refund
            {
                BookingId = booking.BookingId,
                Amount = 100,
                Status = "Approved",
                ProcessedById = processor.Id
            };

            var added = await _repo.AddAsync(refund);

            Assert.That(added.RefundId, Is.GreaterThan(0));
            Assert.That(added.Amount, Is.EqualTo(100));
            Assert.That(added.Booking, Is.Not.Null);
            Assert.That(added.ProcessedUser, Is.Not.Null);
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsRefund()
        {
            var booking = SeedBooking();
            var refund = new Refund { BookingId = booking.BookingId, Amount = 200, Status = "Pending" };
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            var found = await _repo.GetByIdAsync(refund.RefundId);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.Amount, Is.EqualTo(200));
        }

        [Test]
        public async Task GetByBookingIdAsync_ShouldReturnRefunds()
        {
            var booking = SeedBooking();
            _context.Refunds.Add(new Refund { BookingId = booking.BookingId, Amount = 50, Status = "Pending" });
            _context.Refunds.Add(new Refund { BookingId = booking.BookingId, Amount = 75, Status = "Approved" });
            await _context.SaveChangesAsync();

            var refunds = await _repo.GetByBookingIdAsync(booking.BookingId);

            Assert.That(refunds.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateRefund()
        {
            var booking = SeedBooking();
            var refund = new Refund { BookingId = booking.BookingId, Amount = 300, Status = "Pending" };
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            refund.Amount = 450;
            refund.Status = "Processed";

            var updated = await _repo.UpdateAsync(refund.RefundId, refund);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Amount, Is.EqualTo(450));
            Assert.That(updated.Status, Is.EqualTo("Processed"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveRefund()
        {
            var booking = SeedBooking();
            var refund = new Refund { BookingId = booking.BookingId, Amount = 400, Status = "Cancelled" };
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteAsync(refund.RefundId);

            Assert.That(result, Is.True);
            Assert.That(await _repo.GetByIdAsync(refund.RefundId), Is.Null);
        }
    }
}

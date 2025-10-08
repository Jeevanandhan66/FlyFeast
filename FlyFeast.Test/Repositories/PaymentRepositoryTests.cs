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
    public class PaymentRepositoryTests
    {
        private ApplicationDbContext _context;
        private PaymentRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"PaymentDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new PaymentRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private Booking SeedBooking()
        {
            var user = new ApplicationUser { Id = "U1", UserName = "test@test.com", Email = "test@test.com" };
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
                Status = BookingStatus.Pending
            };

            _context.Users.Add(user);
            _context.Schedules.Add(schedule);
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking;
        }

        [Test]
        public async Task AddAsync_ShouldAddPayment()
        {
            var booking = SeedBooking();

            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = 500,
                Provider = "Stripe",
                Status = "Success"
            };

            var added = await _repo.AddAsync(payment);

            Assert.That(added.PaymentId, Is.GreaterThan(0));
            Assert.That(added.Amount, Is.EqualTo(500));
            Assert.That(added.Booking, Is.Not.Null);
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsPayment()
        {
            var booking = SeedBooking();
            var payment = new Payment { BookingId = booking.BookingId, Amount = 200, Provider = "PayPal", Status = "Pending" };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var found = await _repo.GetByIdAsync(payment.PaymentId);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.Amount, Is.EqualTo(200));
        }

        [Test]
        public async Task GetByBookingIdAsync_ShouldReturnPayments()
        {
            var booking = SeedBooking();
            _context.Payments.Add(new Payment { BookingId = booking.BookingId, Amount = 100, Provider = "Test", Status = "Done" });
            _context.Payments.Add(new Payment { BookingId = booking.BookingId, Amount = 150, Provider = "Test", Status = "Done" });
            await _context.SaveChangesAsync();

            var payments = await _repo.GetByBookingIdAsync(booking.BookingId);

            Assert.That(payments.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdatePayment()
        {
            var booking = SeedBooking();
            var payment = new Payment { BookingId = booking.BookingId, Amount = 300, Provider = "Old", Status = "Pending" };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            payment.Amount = 600;
            payment.Provider = "Updated";

            var updated = await _repo.UpdateAsync(payment.PaymentId, payment);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.Amount, Is.EqualTo(600));
            Assert.That(updated.Provider, Is.EqualTo("Updated"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemovePayment()
        {
            var booking = SeedBooking();
            var payment = new Payment { BookingId = booking.BookingId, Amount = 400, Provider = "ToDelete", Status = "Done" };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteAsync(payment.PaymentId);

            Assert.That(result, Is.True);
            Assert.That(await _repo.GetByIdAsync(payment.PaymentId), Is.Null);
        }
    }
}

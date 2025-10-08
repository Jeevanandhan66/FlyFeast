using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class SeatRepositoryTests
    {
        private ApplicationDbContext _context;
        private SeatRepository _repo;
        private Schedule _schedule;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"SeatDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new SeatRepository(_context);

            // Seed schedule (needed for Seat.ScheduleId foreign key)
            _schedule = new Schedule
            {
                DepartureTime = DateTime.Now.AddHours(1),
                ArrivalTime = DateTime.Now.AddHours(3),
                Status = ScheduleStatus.Scheduled,
                SeatCapacity = 5,
                AvailableSeats = 5
            };

            _context.Schedules.Add(_schedule);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddSeat()
        {
            var seat = new Seat
            {
                SeatNumber = "A1",
                Class = SeatClass.Economy,
                Price = 100,
                ScheduleId = _schedule.ScheduleId
            };

            var result = await _repo.AddAsync(seat);

            Assert.That(result.SeatId, Is.GreaterThan(0));
            Assert.That(result.SeatNumber, Is.EqualTo("A1"));
            Assert.That(result.ScheduleId, Is.EqualTo(_schedule.ScheduleId));
        }

        [Test]
        public void AddAsync_WhenDuplicateSeat_ThrowsException()
        {
            var seat1 = new Seat { SeatNumber = "B1", Class = SeatClass.Economy, Price = 120, ScheduleId = _schedule.ScheduleId };
            var seat2 = new Seat { SeatNumber = "B1", Class = SeatClass.Business, Price = 200, ScheduleId = _schedule.ScheduleId };

            _context.Seats.Add(seat1);
            _context.SaveChanges();

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _repo.AddAsync(seat2));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnSeats()
        {
            _context.Seats.AddRange(
                new Seat { SeatNumber = "C1", Class = SeatClass.Economy, Price = 100, ScheduleId = _schedule.ScheduleId },
                new Seat { SeatNumber = "C2", Class = SeatClass.Business, Price = 200, ScheduleId = _schedule.ScheduleId }
            );
            await _context.SaveChangesAsync();

            var result = await _repo.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsSeat()
        {
            var seat = new Seat { SeatNumber = "D1", Class = SeatClass.First, Price = 500, ScheduleId = _schedule.ScheduleId };
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            var found = await _repo.GetByIdAsync(seat.SeatId);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.SeatNumber, Is.EqualTo("D1"));
        }

        [Test]
        public async Task GetByScheduleAsync_ShouldReturnSeatsForSchedule()
        {
            var seat = new Seat { SeatNumber = "E1", Class = SeatClass.Economy, Price = 150, ScheduleId = _schedule.ScheduleId };
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByScheduleAsync(_schedule.ScheduleId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].SeatNumber, Is.EqualTo("E1"));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateSeat()
        {
            var seat = new Seat { SeatNumber = "F1", Class = SeatClass.Economy, Price = 100, ScheduleId = _schedule.ScheduleId };
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            seat.SeatNumber = "F2";
            seat.Price = 200;

            var updated = await _repo.UpdateAsync(seat.SeatId, seat);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.SeatNumber, Is.EqualTo("F2"));
            Assert.That(updated.Price, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveSeat()
        {
            var seat = new Seat { SeatNumber = "G1", Class = SeatClass.Business, Price = 300, ScheduleId = _schedule.ScheduleId };
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteAsync(seat.SeatId);

            Assert.That(result, Is.True);
            Assert.That(await _repo.GetByIdAsync(seat.SeatId), Is.Null);
        }
    }
}

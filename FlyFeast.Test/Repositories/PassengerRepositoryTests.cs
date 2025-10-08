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
    public class PassengerRepositoryTests
    {
        private ApplicationDbContext _context;
        private PassengerRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"PassengerDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new PassengerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private ApplicationUser SeedUser(string id = "U1")
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = $"{id}@test.com",
                Email = $"{id}@test.com"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        [Test]
        public async Task AddAsync_ShouldAddPassenger()
        {
            var user = SeedUser();

            var passenger = new Passenger
            {
                UserId = user.Id,
                DateOfBirth = new DateTime(1990, 1, 1),
                PassportNumber = "P12345",
                Nationality = "Testland"
            };

            var added = await _repo.AddAsync(passenger);

            Assert.That(added.PassengerId, Is.GreaterThan(0));
            Assert.That(added.UserId, Is.EqualTo(user.Id));
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsPassenger()
        {
            var user = SeedUser();
            var passenger = new Passenger { UserId = user.Id, PassportNumber = "XYZ", Nationality = "Demo" };
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            var found = await _repo.GetByIdAsync(passenger.PassengerId);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.PassportNumber, Is.EqualTo("XYZ"));
        }

        [Test]
        public async Task GetPassengerByUserId_WhenExists_ReturnsPassenger()
        {
            var user = SeedUser("U2");
            var passenger = new Passenger { UserId = user.Id, Nationality = "Alpha" };
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            var found = await _repo.GetPassengerByUserId(user.Id);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.Nationality, Is.EqualTo("Alpha"));
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyPassenger()
        {
            var user = SeedUser("U3");
            var passenger = new Passenger { UserId = user.Id, PassportNumber = "OLD" };
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            passenger.PassportNumber = "NEW";
            var updated = await _repo.UpdateAsync(passenger.PassengerId, passenger);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.PassportNumber, Is.EqualTo("NEW"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemovePassenger()
        {
            var user = SeedUser("U4");
            var passenger = new Passenger { UserId = user.Id, PassportNumber = "DEL" };
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            var result = await _repo.DeleteAsync(passenger.PassengerId);

            Assert.That(result, Is.True);
            Assert.That(await _repo.GetByIdAsync(passenger.PassengerId), Is.Null);
        }
    }
}

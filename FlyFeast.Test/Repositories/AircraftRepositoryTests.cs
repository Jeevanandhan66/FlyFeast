using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class AircraftRepositoryTests
    {
        private ApplicationDbContext _context;
        private AircraftRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new AircraftRepository(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddAircraft()
        {
            var aircraft = new Aircraft { AircraftCode = "AC123", AircraftName = "Test Plane" };

            var created = await _repo.AddAsync(aircraft);

            Assert.That(created.AircraftId, Is.GreaterThan(0));
            Assert.That(await _context.Aircrafts.CountAsync(), Is.EqualTo(1));
        }
        [Test]
        public async Task GetAllAsync_ShouldReturnAircrafts()
        {

            var owner = new ApplicationUser { Id = "o1", UserName = "owner@test.com", Email = "owner@test.com" };
            _context.Users.Add(owner);
            await _context.SaveChangesAsync();


            await _repo.AddAsync(new Aircraft { AircraftCode = "AC1", AircraftName = "Plane1", OwnerId = "o1" });
            await _repo.AddAsync(new Aircraft { AircraftCode = "AC2", AircraftName = "Plane2", OwnerId = "o1" });

            var result = await _repo.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsAircraft()
        {

            var owner = new ApplicationUser { Id = "o2", UserName = "owner2@test.com", Email = "owner2@test.com" };
            _context.Users.Add(owner);
            await _context.SaveChangesAsync();


            var added = await _repo.AddAsync(new Aircraft
            {
                AircraftCode = "AC9",
                AircraftName = "Test9",
                OwnerId = "o2"
            });
            var found = await _repo.GetByIdAsync(added.AircraftId);
            Assert.That(found, Is.Not.Null);
            Assert.That(found!.AircraftName, Is.EqualTo("Test9"));
            Assert.That(found.Owner, Is.Not.Null);
            Assert.That(found.Owner.Id, Is.EqualTo("o2"));
        }


        [Test]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
        {
            var found = await _repo.GetByIdAsync(999);

            Assert.That(found, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_WhenExists_UpdatesFields()
        {
            var added = await _repo.AddAsync(new Aircraft { AircraftCode = "AC10", AircraftName = "OldName" });

            var updatedData = new Aircraft { AircraftCode = "AC11", AircraftName = "NewName" };

            var updated = await _repo.UpdateAsync(added.AircraftId, updatedData);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.AircraftName, Is.EqualTo("NewName"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotExists_ReturnsNull()
        {
            var updated = await _repo.UpdateAsync(12345, new Aircraft { AircraftCode = "XX" });

            Assert.That(updated, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_ReturnsTrue()
        {
            var added = await _repo.AddAsync(new Aircraft { AircraftCode = "AC5", AircraftName = "ToDelete" });

            var result = await _repo.DeleteAsync(added.AircraftId);

            Assert.That(result, Is.True);
            Assert.That(await _context.Aircrafts.FindAsync(added.AircraftId), Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
        {
            var result = await _repo.DeleteAsync(888);

            Assert.That(result, Is.False);
        }
    }
}

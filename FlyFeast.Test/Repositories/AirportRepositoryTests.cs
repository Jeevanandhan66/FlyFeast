using FlyFeast.API.Data;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class AirportRepositoryTests
    {
        private ApplicationDbContext _context;
        private AirportRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new AirportRepository(_context);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddAirport()
        {
            var airport = new Airport { AirportName = "Test Airport", Code = "TST", City = "TestCity", Country = "TestCountry" };

            var created = await _repo.AddAsync(airport);

            Assert.That(created.AirportId, Is.GreaterThan(0));
            Assert.That(await _context.Airports.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAirports()
        {
            await _repo.AddAsync(new Airport { AirportName = "A1", Code = "C1", City = "City1", Country = "Country1" });
            await _repo.AddAsync(new Airport { AirportName = "A2", Code = "C2", City = "City2", Country = "Country2" });

            var result = await _repo.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_WhenExists_ReturnsAirport()
        {
            var added = await _repo.AddAsync(new Airport { AirportName = "FindMe", Code = "FM", City = "SomeCity", Country = "SomeCountry" });

            var found = await _repo.GetByIdAsync(added.AirportId);

            Assert.That(found, Is.Not.Null);
            Assert.That(found!.AirportName, Is.EqualTo("FindMe"));
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
            var added = await _repo.AddAsync(new Airport { AirportName = "OldName", Code = "ON", City = "OC", Country = "OY" });

            var updatedData = new Airport { AirportName = "NewName", Code = "NN", City = "NC", Country = "NY" };

            var updated = await _repo.UpdateAsync(added.AirportId, updatedData);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated!.AirportName, Is.EqualTo("NewName"));
            Assert.That(updated.Code, Is.EqualTo("NN"));
        }

        [Test]
        public async Task UpdateAsync_WhenNotExists_ReturnsNull()
        {
            var updated = await _repo.UpdateAsync(12345, new Airport { AirportName = "Ghost", Code = "GX" });

            Assert.That(updated, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_ReturnsTrue()
        {
            var added = await _repo.AddAsync(new Airport { AirportName = "DeleteMe", Code = "DM", City = "DC", Country = "DY" });

            var result = await _repo.DeleteAsync(added.AirportId);

            Assert.That(result, Is.True);
            Assert.That(await _context.Airports.FindAsync(added.AirportId), Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WhenNotExists_ReturnsFalse()
        {
            var result = await _repo.DeleteAsync(888);

            Assert.That(result, Is.False);
        }
    }
}

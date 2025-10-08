using FlyFeast.API.Data;
using FlyFeast.API.Data.Repositories;
using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Repositories
{
    [TestFixture]
    public class AdminRepositoryTests
    {
        private ServiceProvider _sp;
        private AdminRepository _repo;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddDataProtection();

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            _sp = services.BuildServiceProvider();

            var ctx = _sp.GetRequiredService<ApplicationDbContext>();
            var userMgr = _sp.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = _sp.GetRequiredService<RoleManager<IdentityRole>>();

            _repo = new AdminRepository(ctx, userMgr, roleMgr);
        }

        [TearDown]
        public void Cleanup()
        {
            _sp?.Dispose();
        }

        [Test]
        public async Task AddUserAsync_ShouldCreateUser_And_AssignRole()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "test@example.com",
                Email = "test@example.com",
                FullName = "Test User",
                IsActive = true
            };

            // Act
            var created = await _repo.AddUserAsync(user, "Password123!", new[] { "Admin" });

            // Assert
            var userMgr = _sp.GetRequiredService<UserManager<ApplicationUser>>();
            var fromDb = await userMgr.FindByEmailAsync("test@example.com");
            var roles = await userMgr.GetRolesAsync(fromDb!);

            Assert.That(fromDb, Is.Not.Null);
            Assert.That(roles, Contains.Item("Admin"));
        }

        [Test]
        public async Task GetUsersAsync_ShouldReturnUsers()
        {
            await _repo.AddUserAsync(new ApplicationUser
            {
                UserName = "one@example.com",
                Email = "one@example.com"
            }, "Password123!", new[] { "Customer" });

            var users = await _repo.GetUsersAsync();

            Assert.That(users.Count, Is.EqualTo(1));
            Assert.That(users[0].Email, Is.EqualTo("one@example.com"));
        }

        [Test]
        public async Task ToggleUserActiveAsync_ShouldFlipFlag()
        {
            var user = new ApplicationUser
            {
                UserName = "two@example.com",
                Email = "two@example.com",
                IsActive = true
            };

            var created = await _repo.AddUserAsync(user, "Password123!", new[] { "Customer" });

            var ok = await _repo.ToggleUserActiveAsync(created.Id, false);

            var userMgr = _sp.GetRequiredService<UserManager<ApplicationUser>>();
            var updated = await userMgr.FindByIdAsync(created.Id);

            Assert.That(ok, Is.True);
            Assert.That(updated!.IsActive, Is.False);
        }
    }
}

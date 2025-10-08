using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.Data.Repositories;
using FlyFeast.API.DTOs.User_Role;
using FlyFeast.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminRepository> _mockAdminRepo;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<IMapper> _mockMapper;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAdminRepo = new Mock<IAdminRepository>();

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);

            _mockMapper = new Mock<IMapper>();

            _controller = new AdminController(
                _mockAdminRepo.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object,
                _mockMapper.Object
            );
        }

        [Test]
        public async Task GetUserById_WhenUserExists_ReturnsOk()
        {
            // Arrange
            var user = new ApplicationUser { Id = "123", UserName = "testuser" };
            var dto = new UserResponseDTO { Id = "123", UserName = "testuser" };

            _mockAdminRepo.Setup(r => r.GetUserByIdAsync("123"))
                          .ReturnsAsync(user);

            _mockMapper.Setup(m => m.Map<UserResponseDTO>(user))
                       .Returns(dto);

            // Act
            var result = await _controller.GetUserById("123") as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task GetUserById_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockAdminRepo.Setup(r => r.GetUserByIdAsync("999"))
                          .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.GetUserById("999");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task AddRole_WhenCalled_ReturnsOkWithRole()
        {
            // Arrange
            var dto = new RoleDTO { Name = "Admin" };
            var role = new IdentityRole("Admin") { Id = "1" };

            _mockAdminRepo.Setup(r => r.AddRoleAsync("Admin"))
                          .ReturnsAsync(role);

            // Act
            var result = await _controller.AddRole(dto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var roleResult = result!.Value as RoleDTO;
            Assert.That(roleResult!.Id, Is.EqualTo("1"));
            Assert.That(roleResult.Name, Is.EqualTo("Admin"));
        }
    }
}

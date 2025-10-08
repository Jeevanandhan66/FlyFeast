using FlyFeast.API.Configuration;
using FlyFeast.API.Controllers;
using FlyFeast.API.Data;
using FlyFeast.API.DTOs;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<ITokenService> _mockTokenService;
        private JwtSettings _jwtSettings;
        private Mock<ApplicationDbContext> _mockContext;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();
            _jwtSettings = new JwtSettings { DurationInMinutes = 60, Key = "dummy", Issuer = "test", Audience = "test" };

            var options = new DbContextOptions<ApplicationDbContext>();
            _mockContext = new Mock<ApplicationDbContext>(options);

            var optionsMock = new Mock<IOptions<JwtSettings>>();
            optionsMock.Setup(o => o.Value).Returns(_jwtSettings);

            _controller = new AuthController(
                _mockUserManager.Object,
                _mockTokenService.Object,
                optionsMock.Object,
                _mockContext.Object
            );
        }

        [Test]
        public async Task Register_WhenPasswordsDoNotMatch_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@test.com",
                Password = "pass123",
                ConfirmPassword = "wrongpass"
            };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Login_WhenUserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto { Email = "nouser@test.com", Password = "123" };
            _mockUserManager.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Login_WhenPasswordInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@test.com", UserName = "test" };
            var dto = new LoginDto { Email = "test@test.com", Password = "wrong" };

            _mockUserManager.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Login_WhenValid_ReturnsOkWithToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", FullName = "Test User", Email = "test@test.com" };
            var dto = new LoginDto { Email = "test@test.com", Password = "correct" };

            _mockUserManager.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });

            _mockTokenService.Setup(t => t.CreateToken(user, It.IsAny<IList<string>>())).Returns("mocked_token");

            // Act
            var result = await _controller.Login(dto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var response = result!.Value as AuthResponseDTO;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Token, Is.EqualTo("mocked_token"));
            Assert.That(response.Role, Is.EqualTo("Customer"));
        }
    }
}

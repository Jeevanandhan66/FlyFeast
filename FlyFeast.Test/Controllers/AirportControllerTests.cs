using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Aircraft_Airport;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class AirportControllerTests
    {
        private Mock<IAirportRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private AirportController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IAirportRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new AirportController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAirports_WhenCalled_ReturnsOkWithDtos()
        {
            // Arrange
            var airports = new List<Airport>
            {
                new Airport { AirportId = 1, AirportName = "Chennai Intl", City = "Chennai", Code = "MAA" }
            };
            var dtos = new List<AirportDTO>
            {
                new AirportDTO { AirportId = 1, AirportName = "Chennai Intl", City = "Chennai", Code = "MAA" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);
            _mockMapper.Setup(m => m.Map<List<AirportDTO>>(airports)).Returns(dtos);

            // Act
            var result = await _controller.GetAirports(null) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(dtos));
        }

        [Test]
        public async Task GetAirport_WhenFound_ReturnsOk()
        {
            // Arrange
            var airport = new Airport { AirportId = 1, AirportName = "Mumbai Intl", City = "Mumbai", Code = "BOM" };
            var dto = new AirportDTO { AirportId = 1, AirportName = "Mumbai Intl", City = "Mumbai", Code = "BOM" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(airport);
            _mockMapper.Setup(m => m.Map<AirportDTO>(airport)).Returns(dto);

            // Act
            var result = await _controller.GetAirport(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task GetAirport_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Airport?)null);

            // Act
            var result = await _controller.GetAirport(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateAirport_WhenValid_ReturnsCreatedAtAction()
        {
            // Arrange
            var requestDto = new AirportDTO { AirportId = 5, AirportName = "Delhi Intl", City = "Delhi", Code = "DEL" };
            var airport = new Airport { AirportId = 5, AirportName = "Delhi Intl", City = "Delhi", Code = "DEL" };
            var responseDto = new AirportDTO { AirportId = 5, AirportName = "Delhi Intl", City = "Delhi", Code = "DEL" };

            _mockMapper.Setup(m => m.Map<Airport>(requestDto)).Returns(airport);
            _mockRepo.Setup(r => r.AddAsync(airport)).ReturnsAsync(airport);
            _mockMapper.Setup(m => m.Map<AirportDTO>(airport)).Returns(responseDto);

            // Act
            var result = await _controller.CreateAirport(requestDto) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetAirport"));
            var created = result.Value as AirportDTO;
            Assert.That(created!.AirportId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateAirport_WhenFound_ReturnsOk()
        {
            // Arrange
            var requestDto = new AirportDTO { AirportId = 10, AirportName = "Hyd Intl", City = "Hyderabad", Code = "HYD" };
            var airport = new Airport { AirportId = 10, AirportName = "Hyd Intl", City = "Hyderabad", Code = "HYD" };
            var responseDto = new AirportDTO { AirportId = 10, AirportName = "Hyd Intl", City = "Hyderabad", Code = "HYD" };

            _mockMapper.Setup(m => m.Map<Airport>(requestDto)).Returns(airport);
            _mockRepo.Setup(r => r.UpdateAsync(10, airport)).ReturnsAsync(airport);
            _mockMapper.Setup(m => m.Map<AirportDTO>(airport)).Returns(responseDto);

            // Act
            var result = await _controller.UpdateAirport(10, requestDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(responseDto));
        }

        [Test]
        public async Task UpdateAirport_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var requestDto = new AirportDTO { AirportId = 99, AirportName = "Ghost Airport", City = "Nowhere", Code = "NUL" };
            var airport = new Airport { AirportId = 99, AirportName = "Ghost Airport", City = "Nowhere", Code = "NUL" };

            _mockMapper.Setup(m => m.Map<Airport>(requestDto)).Returns(airport);
            _mockRepo.Setup(r => r.UpdateAsync(99, airport)).ReturnsAsync((Airport?)null);

            // Act
            var result = await _controller.UpdateAirport(99, requestDto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteAirport_WhenSuccess_ReturnsNoContent()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAirport(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteAirport_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(42)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAirport(42);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

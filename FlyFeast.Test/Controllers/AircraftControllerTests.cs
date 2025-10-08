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
    public class AircraftControllerTests
    {
        private Mock<IAircraftRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private AircraftController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IAircraftRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new AircraftController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAircrafts_WhenCalled_ReturnsOkWithDtos()
        {
            // Arrange
            var aircrafts = new List<Aircraft> { new Aircraft { AircraftId = 1, AircraftName = "Boeing 737" } };
            var dtos = new List<AircraftResponseDTO> { new AircraftResponseDTO { AircraftId = 1, AircraftName = "Boeing 737" } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(aircrafts);
            _mockMapper.Setup(m => m.Map<List<AircraftResponseDTO>>(aircrafts)).Returns(dtos);

            // Act
            var result = await _controller.GetAircrafts() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(dtos));
        }

        [Test]
        public async Task GetAircraft_WhenFound_ReturnsOk()
        {
            // Arrange
            var aircraft = new Aircraft { AircraftId = 1, AircraftName = "Airbus A320" };
            var dto = new AircraftResponseDTO { AircraftId = 1, AircraftName = "Airbus A320" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(aircraft);
            _mockMapper.Setup(m => m.Map<AircraftResponseDTO>(aircraft)).Returns(dto);

            // Act
            var result = await _controller.GetAircraft(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task GetAircraft_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aircraft?)null);

            // Act
            var result = await _controller.GetAircraft(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateAircraft_WhenValid_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new AircraftRequestDTO { AircraftName = "Embraer 190" };
            var aircraft = new Aircraft { AircraftId = 5, AircraftName = "Embraer 190" };
            var responseDto = new AircraftResponseDTO { AircraftId = 5, AircraftName = "Embraer 190" };

            _mockMapper.Setup(m => m.Map<Aircraft>(request)).Returns(aircraft);
            _mockRepo.Setup(r => r.AddAsync(aircraft)).ReturnsAsync(aircraft);
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(aircraft);
            _mockMapper.Setup(m => m.Map<AircraftResponseDTO>(aircraft)).Returns(responseDto);

            // Act
            var result = await _controller.CreateAircraft(request) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetAircraft"));
            Assert.That(((AircraftResponseDTO)result.Value!).AircraftId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateAircraft_WhenFound_ReturnsOk()
        {
            // Arrange
            var request = new AircraftRequestDTO { AircraftName = "Updated Plane" };
            var existing = new Aircraft { AircraftId = 10, AircraftName = "Old Plane" };
            var updated = new Aircraft { AircraftId = 10, AircraftName = "Updated Plane" };
            var dto = new AircraftResponseDTO { AircraftId = 10, AircraftName = "Updated Plane" };

            _mockRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(10, existing)).ReturnsAsync(updated);
            _mockMapper.Setup(m => m.Map(request, existing)).Verifiable();
            _mockMapper.Setup(m => m.Map<AircraftResponseDTO>(updated)).Returns(dto);

            // Act
            var result = await _controller.UpdateAircraft(10, request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task UpdateAircraft_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aircraft?)null);

            // Act
            var result = await _controller.UpdateAircraft(99, new AircraftRequestDTO());

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteAircraft_WhenSuccess_ReturnsNoContent()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAircraft(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteAircraft_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAircraft(99);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

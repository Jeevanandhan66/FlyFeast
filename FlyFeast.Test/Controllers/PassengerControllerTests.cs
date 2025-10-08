using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.User_Role;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class PassengerControllerTests
    {
        private Mock<IPassengerRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private PassengerController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPassengerRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PassengerController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetPassengers_ReturnsOk()
        {
            var passengers = new List<Passenger> { new Passenger { PassengerId = 1 } };
            var dtos = new List<PassengerDTO> { new PassengerDTO { PassengerId = 1 } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(passengers);
            _mockMapper.Setup(m => m.Map<List<PassengerDTO>>(passengers)).Returns(dtos);

            var result = await _controller.GetPassengers() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<PassengerDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPassenger_WhenFound_ReturnsOk()
        {
            var passenger = new Passenger { PassengerId = 2 };
            var dto = new PassengerDTO { PassengerId = 2 };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(passenger);
            _mockMapper.Setup(m => m.Map<PassengerDTO>(passenger)).Returns(dto);

            var result = await _controller.GetPassenger(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((PassengerDTO)result!.Value!).PassengerId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPassenger_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Passenger?)null);

            var result = await _controller.GetPassenger(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetPassengerByUserId_WhenFound_ReturnsOk()
        {
            var passenger = new Passenger { PassengerId = 3, UserId = "user1" };
            var dto = new PassengerDTO { PassengerId = 3, UserId = "user1" };

            _mockRepo.Setup(r => r.GetPassengerByUserId("user1")).ReturnsAsync(passenger);
            _mockMapper.Setup(m => m.Map<PassengerDTO>(passenger)).Returns(dto);

            var result = await _controller.GetPassengerByUserId("user1") as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((PassengerDTO)result!.Value!).PassengerId, Is.EqualTo(3));
        }

        [Test]
        public async Task GetPassengerByUserId_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetPassengerByUserId("unknown")).ReturnsAsync((Passenger?)null);

            var result = await _controller.GetPassengerByUserId("unknown");

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreatePassenger_WhenValid_ReturnsCreatedAtAction()
        {
            var requestDto = new PassengerRequestDTO { UserId = "user1", PassportNumber = "P123" };
            var passenger = new Passenger { PassengerId = 4, UserId = "user1", PassportNumber = "P123" };
            var responseDto = new PassengerDTO { PassengerId = 4, UserId = "user1", PassportNumber = "P123" };

            _mockMapper.Setup(m => m.Map<Passenger>(requestDto)).Returns(passenger);
            _mockRepo.Setup(r => r.AddAsync(passenger)).ReturnsAsync(passenger);
            _mockMapper.Setup(m => m.Map<PassengerDTO>(passenger)).Returns(responseDto);

            var result = await _controller.CreatePassenger(requestDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetPassenger"));
            Assert.That(((PassengerDTO)result.Value!).PassengerId, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdatePassenger_WhenFound_ReturnsOk()
        {
            var requestDto = new PassengerRequestDTO { UserId = "user2", Nationality = "Indian" };
            var passenger = new Passenger { PassengerId = 5, UserId = "user2", Nationality = "Indian" };
            var responseDto = new PassengerDTO { PassengerId = 5, UserId = "user2", Nationality = "Indian" };

            _mockMapper.Setup(m => m.Map<Passenger>(requestDto)).Returns(passenger);
            _mockRepo.Setup(r => r.UpdateAsync(5, passenger)).ReturnsAsync(passenger);
            _mockMapper.Setup(m => m.Map<PassengerDTO>(passenger)).Returns(responseDto);

            var result = await _controller.UpdatePassenger(5, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((PassengerDTO)result!.Value!).PassengerId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdatePassenger_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new PassengerRequestDTO { UserId = "userX" };
            var passenger = new Passenger { PassengerId = 6 };

            _mockMapper.Setup(m => m.Map<Passenger>(requestDto)).Returns(passenger);
            _mockRepo.Setup(r => r.UpdateAsync(6, passenger)).ReturnsAsync((Passenger?)null);

            var result = await _controller.UpdatePassenger(6, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeletePassenger_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _controller.DeletePassenger(7);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeletePassenger_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(8)).ReturnsAsync(false);

            var result = await _controller.DeletePassenger(8);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Routes;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class FlightRouteControllerTests
    {
        private Mock<IFlightRouteRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private FlightRouteController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IFlightRouteRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new FlightRouteController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetRoutes_ReturnsOk()
        {
            var routes = new List<FlightRoute> { new FlightRoute { RouteId = 1 } };
            var dtos = new List<RouteResponseDTO> { new RouteResponseDTO { RouteId = 1 } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(routes);
            _mockMapper.Setup(m => m.Map<List<RouteResponseDTO>>(routes)).Returns(dtos);

            var result = await _controller.GetRoutes() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<RouteResponseDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetRoute_WhenFound_ReturnsOk()
        {
            var route = new FlightRoute { RouteId = 2 };
            var dto = new RouteResponseDTO { RouteId = 2 };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(route);
            _mockMapper.Setup(m => m.Map<RouteResponseDTO>(route)).Returns(dto);

            var result = await _controller.GetRoute(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((RouteResponseDTO)result!.Value!).RouteId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetRoute_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FlightRoute?)null);

            var result = await _controller.GetRoute(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateRoute_WhenValid_ReturnsCreatedAtAction()
        {
            var requestDto = new RouteRequestDTO { OriginAirportId = 1, DestinationAirportId = 2 };
            var route = new FlightRoute { RouteId = 3 };
            var responseDto = new RouteResponseDTO { RouteId = 3 };

            _mockMapper.Setup(m => m.Map<FlightRoute>(requestDto)).Returns(route);
            _mockRepo.Setup(r => r.AddAsync(route)).ReturnsAsync(route);
            _mockMapper.Setup(m => m.Map<RouteResponseDTO>(route)).Returns(responseDto);

            var result = await _controller.CreateRoute(requestDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetRoute"));
            Assert.That(((RouteResponseDTO)result.Value!).RouteId, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateRoute_WhenFound_ReturnsOk()
        {
            var requestDto = new RouteRequestDTO { OriginAirportId = 1, DestinationAirportId = 2 };
            var route = new FlightRoute { RouteId = 4 };
            var responseDto = new RouteResponseDTO { RouteId = 4 };

            _mockMapper.Setup(m => m.Map<FlightRoute>(requestDto)).Returns(route);
            _mockRepo.Setup(r => r.UpdateAsync(4, route)).ReturnsAsync(route);
            _mockMapper.Setup(m => m.Map<RouteResponseDTO>(route)).Returns(responseDto);

            var result = await _controller.UpdateRoute(4, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((RouteResponseDTO)result!.Value!).RouteId, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdateRoute_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new RouteRequestDTO { OriginAirportId = 5, DestinationAirportId = 6 };
            var route = new FlightRoute { RouteId = 99 };

            _mockMapper.Setup(m => m.Map<FlightRoute>(requestDto)).Returns(route);
            _mockRepo.Setup(r => r.UpdateAsync(99, route)).ReturnsAsync((FlightRoute?)null);

            var result = await _controller.UpdateRoute(99, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteRoute_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteRoute(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteRoute_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(2)).ReturnsAsync(false);

            var result = await _controller.DeleteRoute(2);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

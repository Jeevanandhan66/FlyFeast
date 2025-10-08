using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Schedules;
using FlyFeast.API.Models;
using FlyFeast.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class SearchControllerTests
    {
        private Mock<IFlightSearchService> _mockService;
        private Mock<IMapper> _mockMapper;
        private SearchController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IFlightSearchService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new SearchController(_mockService.Object, _mockMapper.Object);
        }

        [Test]
        public async Task SearchFlights_WhenOriginOrDestinationMissing_ReturnsBadRequest()
        {
            var result = await _controller.SearchFlights("", "NYC", DateTime.Now);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task SearchFlights_WhenNoFlightsFound_ReturnsNotFound()
        {
            _mockService.Setup(s =>
                s.SearchFlightsAsync("LAX", "NYC", It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Schedule>());

            var result = await _controller.SearchFlights("LAX", "NYC", DateTime.Now);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task SearchFlights_WhenFlightsFound_ReturnsOk()
        {
            var schedules = new List<Schedule> { new Schedule { ScheduleId = 1, RouteId = 100 } };
            var dtoList = new List<ScheduleResponseDTO>
            {
                new ScheduleResponseDTO
                {
                    ScheduleId = 1,
                    Route = new FlyFeast.API.DTOs.Routes.RouteResponseDTO { RouteId = 100 },
                    DepartureTime = DateTime.Now,
                    ArrivalTime = DateTime.Now.AddHours(2),
                    DurationMinutes = 120,
                    DurationFormatted = "2h",
                    SeatCapacity = 180,
                    Status = "Scheduled"
                }
            };

            _mockService.Setup(s =>
                s.SearchFlightsAsync("LAX", "NYC", It.IsAny<DateTime>()))
                .ReturnsAsync(schedules);

            _mockMapper.Setup(m => m.Map<List<ScheduleResponseDTO>>(schedules)).Returns(dtoList);

            var result = await _controller.SearchFlights("LAX", "NYC", DateTime.Now) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var returned = result!.Value as List<ScheduleResponseDTO>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned![0].ScheduleId, Is.EqualTo(1));
        }
    }
}

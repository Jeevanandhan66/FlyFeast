using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Routes;
using FlyFeast.API.DTOs.Schedules;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class ScheduleControllerTests
    {
        private Mock<IScheduleRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private ScheduleController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IScheduleRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ScheduleController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetSchedules_ReturnsOk()
        {
            var schedules = new List<Schedule> { new Schedule { ScheduleId = 1, RouteId = 10 } };
            var dtos = new List<ScheduleResponseDTO>
            {
                new ScheduleResponseDTO
                {
                    ScheduleId = 1,
                    Route = new RouteResponseDTO { RouteId = 10 },
                    DepartureTime = DateTime.Now,
                    ArrivalTime = DateTime.Now.AddHours(2),
                    DurationMinutes = 120,
                    DurationFormatted = "2h",
                    SeatCapacity = 100,
                    Status = "Scheduled"
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(schedules);
            _mockMapper.Setup(m => m.Map<List<ScheduleResponseDTO>>(schedules)).Returns(dtos);

            var result = await _controller.GetSchedules() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<ScheduleResponseDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetSchedule_WhenFound_ReturnsOk()
        {
            var schedule = new Schedule { ScheduleId = 2, RouteId = 20 };
            var dto = new ScheduleResponseDTO
            {
                ScheduleId = 2,
                Route = new RouteResponseDTO { RouteId = 20 },
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(3),
                DurationMinutes = 180,
                DurationFormatted = "3h",
                SeatCapacity = 120,
                Status = "Scheduled"
            };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(schedule);
            _mockMapper.Setup(m => m.Map<ScheduleResponseDTO>(schedule)).Returns(dto);

            var result = await _controller.GetSchedule(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((ScheduleResponseDTO)result!.Value!).ScheduleId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetSchedule_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Schedule?)null);

            var result = await _controller.GetSchedule(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreateSchedule_WhenValid_ReturnsCreatedAtAction()
        {
            var requestDto = new ScheduleRequestDTO
            {
                RouteId = 30,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(1),
                SeatCapacity = 150,
                Status = "Scheduled"
            };

            var schedule = new Schedule { ScheduleId = 3, RouteId = 30 };
            var responseDto = new ScheduleResponseDTO
            {
                ScheduleId = 3,
                Route = new RouteResponseDTO { RouteId = 30 },
                DepartureTime = requestDto.DepartureTime,
                ArrivalTime = requestDto.ArrivalTime,
                DurationMinutes = 60,
                DurationFormatted = "1h",
                SeatCapacity = 150,
                Status = "Scheduled"
            };

            _mockMapper.Setup(m => m.Map<Schedule>(requestDto)).Returns(schedule);
            _mockRepo.Setup(r => r.AddAsync(schedule)).ReturnsAsync(schedule);
            _mockMapper.Setup(m => m.Map<ScheduleResponseDTO>(schedule)).Returns(responseDto);

            var result = await _controller.CreateSchedule(requestDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetSchedule"));
            Assert.That(((ScheduleResponseDTO)result.Value!).ScheduleId, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateSchedule_WhenFound_ReturnsOk()
        {
            var requestDto = new ScheduleRequestDTO
            {
                RouteId = 40,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(2),
                SeatCapacity = 200,
                Status = "Scheduled"
            };

            var schedule = new Schedule { ScheduleId = 4, RouteId = 40 };
            var responseDto = new ScheduleResponseDTO
            {
                ScheduleId = 4,
                Route = new RouteResponseDTO { RouteId = 40 },
                DepartureTime = requestDto.DepartureTime,
                ArrivalTime = requestDto.ArrivalTime,
                DurationMinutes = 120,
                DurationFormatted = "2h",
                SeatCapacity = 200,
                Status = "Scheduled"
            };

            _mockMapper.Setup(m => m.Map<Schedule>(requestDto)).Returns(schedule);
            _mockRepo.Setup(r => r.UpdateAsync(4, schedule)).ReturnsAsync(schedule);
            _mockMapper.Setup(m => m.Map<ScheduleResponseDTO>(schedule)).Returns(responseDto);

            var result = await _controller.UpdateSchedule(4, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((ScheduleResponseDTO)result!.Value!).ScheduleId, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdateSchedule_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new ScheduleRequestDTO
            {
                RouteId = 50,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(4),
                SeatCapacity = 250,
                Status = "Scheduled"
            };

            var schedule = new Schedule { ScheduleId = 5, RouteId = 50 };

            _mockMapper.Setup(m => m.Map<Schedule>(requestDto)).Returns(schedule);
            _mockRepo.Setup(r => r.UpdateAsync(5, schedule)).ReturnsAsync((Schedule?)null);

            var result = await _controller.UpdateSchedule(5, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteSchedule_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(6)).ReturnsAsync(true);

            var result = await _controller.DeleteSchedule(6);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteSchedule_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(7)).ReturnsAsync(false);

            var result = await _controller.DeleteSchedule(7);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}

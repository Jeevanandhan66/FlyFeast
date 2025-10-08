using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Seats;
using FlyFeast.API.DTOs.Schedules;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class SeatControllerTests
    {
        private Mock<ISeatRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private SeatController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ISeatRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new SeatController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetSeats_ReturnsOk()
        {
            var seats = new List<Seat> { new Seat { SeatId = 1 } };
            var dtos = new List<SeatResponseDTO>
            {
                new SeatResponseDTO
                {
                    SeatId = 1,
                    SeatNumber = "A1",
                    Class = "Economy",
                    Price = 100,
                    Schedule = new ScheduleSummaryDTO { ScheduleId = 10 }
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(seats);
            _mockMapper.Setup(m => m.Map<List<SeatResponseDTO>>(seats)).Returns(dtos);

            var result = await _controller.GetSeats() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<SeatResponseDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetSeat_WhenFound_ReturnsOk()
        {
            var seat = new Seat { SeatId = 2, SeatNumber = "B2" };
            var dto = new SeatResponseDTO
            {
                SeatId = 2,
                SeatNumber = "B2",
                Class = "Business",
                Price = 200,
                Schedule = new ScheduleSummaryDTO { ScheduleId = 20 }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(seat);
            _mockMapper.Setup(m => m.Map<SeatResponseDTO>(seat)).Returns(dto);

            var result = await _controller.GetSeat(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((SeatResponseDTO)result!.Value!).SeatId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetSeat_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Seat?)null);

            var result = await _controller.GetSeat(99);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetSeatsBySchedule_ReturnsOk()
        {
            var seats = new List<Seat> { new Seat { SeatId = 3, ScheduleId = 30 } };
            var dtos = new List<SeatResponseDTO>
            {
                new SeatResponseDTO
                {
                    SeatId = 3,
                    SeatNumber = "C3",
                    Class = "First",
                    Price = 300,
                    Schedule = new ScheduleSummaryDTO { ScheduleId = 30 }
                }
            };

            _mockRepo.Setup(r => r.GetByScheduleAsync(30)).ReturnsAsync(seats);
            _mockMapper.Setup(m => m.Map<List<SeatResponseDTO>>(seats)).Returns(dtos);

            var result = await _controller.GetSeatsBySchedule(30) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var returned = result!.Value as List<SeatResponseDTO>;
            Assert.That(returned![0].Schedule.ScheduleId, Is.EqualTo(30));
        }

        [Test]
        public async Task CreateSeat_WhenValid_ReturnsCreatedAtAction()
        {
            var requestDto = new SeatRequestDTO
            {
                ScheduleId = 40,
                SeatNumber = "D4",
                Class = "Economy",
                Price = 150
            };

            var seat = new Seat { SeatId = 4, SeatNumber = "D4", ScheduleId = 40, Price = 150 };
            var responseDto = new SeatResponseDTO
            {
                SeatId = 4,
                SeatNumber = "D4",
                Class = "Economy",
                Price = 150,
                Schedule = new ScheduleSummaryDTO { ScheduleId = 40 }
            };

            _mockMapper.Setup(m => m.Map<Seat>(requestDto)).Returns(seat);
            _mockRepo.Setup(r => r.AddAsync(seat)).ReturnsAsync(seat);
            _mockMapper.Setup(m => m.Map<SeatResponseDTO>(seat)).Returns(responseDto);

            var result = await _controller.CreateSeat(requestDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetSeat"));
            Assert.That(((SeatResponseDTO)result.Value!).SeatId, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdateSeat_WhenFound_ReturnsOk()
        {
            var requestDto = new SeatRequestDTO
            {
                ScheduleId = 50,
                SeatNumber = "E5",
                Class = "Business",
                Price = 250
            };

            var seat = new Seat { SeatId = 5, ScheduleId = 50, SeatNumber = "E5", Price = 250 };
            var responseDto = new SeatResponseDTO
            {
                SeatId = 5,
                SeatNumber = "E5",
                Class = "Business",
                Price = 250,
                Schedule = new ScheduleSummaryDTO { ScheduleId = 50 }
            };

            _mockMapper.Setup(m => m.Map<Seat>(requestDto)).Returns(seat);
            _mockRepo.Setup(r => r.UpdateAsync(5, seat)).ReturnsAsync(seat);
            _mockMapper.Setup(m => m.Map<SeatResponseDTO>(seat)).Returns(responseDto);

            var result = await _controller.UpdateSeat(5, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((SeatResponseDTO)result!.Value!).SeatId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateSeat_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new SeatRequestDTO
            {
                ScheduleId = 60,
                SeatNumber = "F6",
                Class = "First",
                Price = 350
            };

            var seat = new Seat { SeatId = 6, ScheduleId = 60, SeatNumber = "F6", Price = 350 };

            _mockMapper.Setup(m => m.Map<Seat>(requestDto)).Returns(seat);
            _mockRepo.Setup(r => r.UpdateAsync(6, seat)).ReturnsAsync((Seat?)null);

            var result = await _controller.UpdateSeat(6, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task DeleteSeat_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _controller.DeleteSeat(7);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteSeat_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(8)).ReturnsAsync(false);

            var result = await _controller.DeleteSeat(8);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}

using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<IBookingRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private BookingController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new BookingController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetBookings_ReturnsOk()
        {
            var bookings = new List<Booking> { new Booking { BookingId = 1 } };
            var dtos = new List<BookingResponseDTO> { new BookingResponseDTO { BookingId = 1 } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(bookings);
            _mockMapper.Setup(m => m.Map<List<BookingResponseDTO>>(bookings)).Returns(dtos);

            var result = await _controller.GetBookings() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<BookingResponseDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetBooking_WhenFound_ReturnsOk()
        {
            var booking = new Booking { BookingId = 2 };
            var dto = new BookingResponseDTO { BookingId = 2 };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(booking);
            _mockMapper.Setup(m => m.Map<BookingResponseDTO>(booking)).Returns(dto);

            var result = await _controller.GetBooking(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((BookingResponseDTO)result!.Value!).BookingId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetBooking_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Booking?)null);

            var result = await _controller.GetBooking(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateBooking_WhenValid_ReturnsOk()
        {
            var requestDto = new BookingRequestDTO
            {
                UserId = "user1",
                ScheduleId = 10,
                Seats = new List<SeatPassengerDTO> { new SeatPassengerDTO { SeatId = 1, PassengerId = 2 } }
            };

            var booking = new Booking { BookingId = 3 };
            var dto = new BookingResponseDTO { BookingId = 3 };

            _mockRepo.Setup(r => r.AddAsync(
                    It.IsAny<Booking>(),
                    It.IsAny<List<(int SeatId, int PassengerId)>>()))
                .ReturnsAsync(booking);

            _mockRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(booking);
            _mockMapper.Setup(m => m.Map<BookingResponseDTO>(booking)).Returns(dto);

            var result = await _controller.CreateBooking(requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((BookingResponseDTO)result!.Value!).BookingId, Is.EqualTo(3));
        }


        [Test]
        public async Task UpdateBooking_WhenFound_ReturnsOk()
        {
            var requestDto = new BookingRequestDTO { UserId = "user1" };
            var booking = new Booking { BookingId = 4 };
            var dto = new BookingResponseDTO { BookingId = 4 };

            _mockMapper.Setup(m => m.Map<Booking>(requestDto)).Returns(booking);
            _mockRepo.Setup(r => r.UpdateAsync(4, booking)).ReturnsAsync(booking);
            _mockMapper.Setup(m => m.Map<BookingResponseDTO>(booking)).Returns(dto);

            var result = await _controller.UpdateBooking(4, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((BookingResponseDTO)result!.Value!).BookingId, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdateBooking_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new BookingRequestDTO { UserId = "user1" };
            var booking = new Booking { BookingId = 5 };

            _mockMapper.Setup(m => m.Map<Booking>(requestDto)).Returns(booking);
            _mockRepo.Setup(r => r.UpdateAsync(5, booking)).ReturnsAsync((Booking?)null);

            var result = await _controller.UpdateBooking(5, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CancelBooking_WhenValid_ReturnsOk()
        {
            var booking = new Booking { BookingId = 6, Status = BookingStatus.Confirmed };

            _mockRepo.Setup(r => r.GetByIdAsync(6)).ReturnsAsync(booking);
            _mockRepo.Setup(r => r.AddCancellationAsync(It.IsAny<BookingCancellation>()))
                     .Returns(Task.CompletedTask);

            var result = await _controller.CancelBooking(6, new BookingCancellationDTO()) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task DeleteBooking_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _controller.DeleteBooking(7);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }
    }
}

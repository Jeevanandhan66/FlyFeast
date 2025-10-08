using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.DTOs.Refunds;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class RefundControllerTests
    {
        private Mock<IRefundRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private RefundController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IRefundRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new RefundController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetRefunds_ReturnsOk()
        {
            var refunds = new List<Refund> { new Refund { RefundId = 1, BookingId = 101 } };
            var dtos = new List<RefundResponseDTO>
            {
                new RefundResponseDTO
                {
                    RefundId = 1,
                    Amount = 500,
                    Booking = new BookingResponseDTO { BookingId = 101 }
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(refunds);
            _mockMapper.Setup(m => m.Map<List<RefundResponseDTO>>(refunds)).Returns(dtos);

            var result = await _controller.GetRefunds() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<RefundResponseDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetRefund_WhenFound_ReturnsOk()
        {
            var refund = new Refund { RefundId = 2, BookingId = 102 };
            var dto = new RefundResponseDTO
            {
                RefundId = 2,
                Amount = 600,
                Booking = new BookingResponseDTO { BookingId = 102 }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(refund);
            _mockMapper.Setup(m => m.Map<RefundResponseDTO>(refund)).Returns(dto);

            var result = await _controller.GetRefund(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((RefundResponseDTO)result!.Value!).RefundId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetRefund_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Refund?)null);

            var result = await _controller.GetRefund(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetRefundsByBooking_ReturnsOk()
        {
            var refunds = new List<Refund> { new Refund { RefundId = 3, BookingId = 200 } };
            var dtos = new List<RefundResponseDTO>
            {
                new RefundResponseDTO
                {
                    RefundId = 3,
                    Amount = 700,
                    Booking = new BookingResponseDTO { BookingId = 200 }
                }
            };

            _mockRepo.Setup(r => r.GetByBookingIdAsync(200)).ReturnsAsync(refunds);
            _mockMapper.Setup(m => m.Map<List<RefundResponseDTO>>(refunds)).Returns(dtos);

            var result = await _controller.GetRefundsByBooking(200) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var returned = result!.Value as List<RefundResponseDTO>;
            Assert.That(returned![0].Booking.BookingId, Is.EqualTo(200));
        }

        [Test]
        public async Task CreateRefund_WhenValid_ReturnsCreatedAtAction()
        {
            var requestDto = new RefundRequestDTO { BookingId = 300, Amount = 1000 };
            var refund = new Refund { RefundId = 4, BookingId = 300, Amount = 1000 };
            var responseDto = new RefundResponseDTO
            {
                RefundId = 4,
                Amount = 1000,
                Booking = new BookingResponseDTO { BookingId = 300 }
            };

            _mockMapper.Setup(m => m.Map<Refund>(requestDto)).Returns(refund);
            _mockRepo.Setup(r => r.AddAsync(refund)).ReturnsAsync(refund);
            _mockMapper.Setup(m => m.Map<RefundResponseDTO>(refund)).Returns(responseDto);

            var result = await _controller.CreateRefund(requestDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetRefund"));
            Assert.That(((RefundResponseDTO)result.Value!).RefundId, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdateRefund_WhenFound_ReturnsOk()
        {
            var requestDto = new RefundRequestDTO { BookingId = 400, Amount = 2000 };
            var refund = new Refund { RefundId = 5, BookingId = 400, Amount = 2000 };
            var refreshed = new Refund { RefundId = 5, BookingId = 400, Amount = 2000 };
            var responseDto = new RefundResponseDTO
            {
                RefundId = 5,
                Amount = 2000,
                Booking = new BookingResponseDTO { BookingId = 400 }
            };

            _mockMapper.Setup(m => m.Map<Refund>(requestDto)).Returns(refund);
            _mockRepo.Setup(r => r.UpdateAsync(5, refund)).ReturnsAsync(refund);
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(refreshed);
            _mockMapper.Setup(m => m.Map<RefundResponseDTO>(refreshed)).Returns(responseDto);

            var result = await _controller.UpdateRefund(5, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((RefundResponseDTO)result!.Value!).RefundId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateRefund_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new RefundRequestDTO { BookingId = 500, Amount = 3000 };
            var refund = new Refund { RefundId = 6, BookingId = 500, Amount = 3000 };

            _mockMapper.Setup(m => m.Map<Refund>(requestDto)).Returns(refund);
            _mockRepo.Setup(r => r.UpdateAsync(6, refund)).ReturnsAsync((Refund?)null);

            var result = await _controller.UpdateRefund(6, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteRefund_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _controller.DeleteRefund(7);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteRefund_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(8)).ReturnsAsync(false);

            var result = await _controller.DeleteRefund(8);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

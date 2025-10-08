using AutoMapper;
using FlyFeast.API.Controllers;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.DTOs.Payments;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FlyFeast.API.Tests.Controllers
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private Mock<IPaymentRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private PaymentController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPaymentRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PaymentController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetPayments_ReturnsOk()
        {
            var payments = new List<Payment> { new Payment { PaymentId = 1 } };
            var dtos = new List<PaymentResponseDTO>
            {
                new PaymentResponseDTO
                {
                    PaymentId = 1,
                    Booking = new BookingResponseDTO { BookingId = 100 }
                }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(payments);
            _mockMapper.Setup(m => m.Map<List<PaymentResponseDTO>>(payments)).Returns(dtos);

            var result = await _controller.GetPayments() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((List<PaymentResponseDTO>)result!.Value!).Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPayment_WhenFound_ReturnsOk()
        {
            var payment = new Payment { PaymentId = 2 };
            var dto = new PaymentResponseDTO
            {
                PaymentId = 2,
                Booking = new BookingResponseDTO { BookingId = 200 }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(payment);
            _mockMapper.Setup(m => m.Map<PaymentResponseDTO>(payment)).Returns(dto);

            var result = await _controller.GetPayment(2) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((PaymentResponseDTO)result!.Value!).PaymentId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetPayment_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Payment?)null);

            var result = await _controller.GetPayment(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetPaymentsByBooking_ReturnsOk()
        {
            var payments = new List<Payment> { new Payment { PaymentId = 3, BookingId = 10 } };
            var dtos = new List<PaymentResponseDTO>
            {
                new PaymentResponseDTO
                {
                    PaymentId = 3,
                    Booking = new BookingResponseDTO { BookingId = 10 }
                }
            };

            _mockRepo.Setup(r => r.GetByBookingIdAsync(10)).ReturnsAsync(payments);
            _mockMapper.Setup(m => m.Map<List<PaymentResponseDTO>>(payments)).Returns(dtos);

            var result = await _controller.GetPaymentsByBooking(10) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var returned = result!.Value as List<PaymentResponseDTO>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned![0].Booking.BookingId, Is.EqualTo(10));
        }

        [Test]
        public async Task CreatePayment_WhenValid_ReturnsCreatedAtAction()
        {
            var requestDto = new PaymentRequestDTO { BookingId = 11, Amount = 200 };
            var payment = new Payment { PaymentId = 4, BookingId = 11, Amount = 200, ProviderRef = "PAY-12345678" };
            var responseDto = new PaymentResponseDTO
            {
                PaymentId = 4,
                Amount = 200,
                Booking = new BookingResponseDTO { BookingId = 11 }
            };

            _mockMapper.Setup(m => m.Map<Payment>(requestDto)).Returns(payment);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Payment>())).ReturnsAsync(payment);
            _mockMapper.Setup(m => m.Map<PaymentResponseDTO>(payment)).Returns(responseDto);

            var result = await _controller.CreatePayment(requestDto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ActionName, Is.EqualTo("GetPayment"));
            var returned = result.Value as PaymentResponseDTO;
            Assert.That(returned!.Booking.BookingId, Is.EqualTo(11));
        }

        [Test]
        public async Task UpdatePayment_WhenFound_ReturnsOk()
        {
            var requestDto = new PaymentRequestDTO { BookingId = 12, Amount = 300 };
            var payment = new Payment { PaymentId = 5, BookingId = 12, Amount = 300 };
            var responseDto = new PaymentResponseDTO
            {
                PaymentId = 5,
                Amount = 300,
                Booking = new BookingResponseDTO { BookingId = 12 }
            };

            _mockMapper.Setup(m => m.Map<Payment>(requestDto)).Returns(payment);
            _mockRepo.Setup(r => r.UpdateAsync(5, payment)).ReturnsAsync(payment);
            _mockMapper.Setup(m => m.Map<PaymentResponseDTO>(payment)).Returns(responseDto);

            var result = await _controller.UpdatePayment(5, requestDto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(((PaymentResponseDTO)result!.Value!).PaymentId, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdatePayment_WhenNotFound_ReturnsNotFound()
        {
            var requestDto = new PaymentRequestDTO { BookingId = 13, Amount = 400 };
            var payment = new Payment { PaymentId = 6, BookingId = 13, Amount = 400 };

            _mockMapper.Setup(m => m.Map<Payment>(requestDto)).Returns(payment);
            _mockRepo.Setup(r => r.UpdateAsync(6, payment)).ReturnsAsync((Payment?)null);

            var result = await _controller.UpdatePayment(6, requestDto);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeletePayment_WhenSuccess_ReturnsNoContent()
        {
            _mockRepo.Setup(r => r.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _controller.DeletePayment(7);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeletePayment_WhenNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(8)).ReturnsAsync(false);

            var result = await _controller.DeletePayment(8);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}

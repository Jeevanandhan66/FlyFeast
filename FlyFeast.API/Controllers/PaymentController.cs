using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,Customer")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentController(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetPayments()
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync();
                return Ok(_mapper.Map<List<PaymentResponseDTO>>(payments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(id);
                if (payment == null) return NotFound();

                return Ok(_mapper.Map<PaymentResponseDTO>(payment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("bybooking/{bookingId}")]
        public async Task<IActionResult> GetPaymentsByBooking(int bookingId)
        {
            try
            {
                var payments = await _paymentRepository.GetByBookingIdAsync(bookingId);
                return Ok(_mapper.Map<List<PaymentResponseDTO>>(payments));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentRequestDTO paymentDto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentDto);
                payment.ProviderRef = $"PAY-{Guid.NewGuid().ToString("N")[..8]}";
                var created = await _paymentRepository.AddAsync(payment);
                return CreatedAtAction(nameof(GetPayment), new { id = created.PaymentId }, _mapper.Map<PaymentResponseDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentRequestDTO paymentDto)
        {
            try
            {
                var payment = _mapper.Map<Payment>(paymentDto);
                var updated = await _paymentRepository.UpdateAsync(id, payment);
                if (updated == null) return NotFound();

                return Ok(_mapper.Map<PaymentResponseDTO>(updated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var success = await _paymentRepository.DeleteAsync(id);
                if (!success) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

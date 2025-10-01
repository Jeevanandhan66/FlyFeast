using AutoMapper;
using FlyFeast.API.DTOs;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,Customer")]
    public class RefundController : ControllerBase
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IMapper _mapper;

        public RefundController(IRefundRepository refundRepository, IMapper mapper)
        {
            _refundRepository = refundRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetRefunds()
        {
            try
            {
                var refunds = await _refundRepository.GetAllAsync();
                return Ok(_mapper.Map<List<RefundResponseDTO>>(refunds));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRefund(int id)
        {
            try
            {
                var refund = await _refundRepository.GetByIdAsync(id);
                if (refund == null) return NotFound();

                return Ok(_mapper.Map<RefundResponseDTO>(refund));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("bybooking/{bookingId}")]
        public async Task<IActionResult> GetRefundsByBooking(int bookingId)
        {
            try
            {
                var refunds = await _refundRepository.GetByBookingIdAsync(bookingId);
                return Ok(_mapper.Map<List<RefundResponseDTO>>(refunds));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRefund(RefundRequestDTO refundDto)
        {
            try
            {
                var refund = _mapper.Map<Refund>(refundDto);
                var created = await _refundRepository.AddAsync(refund);
                return CreatedAtAction(nameof(GetRefund), new { id = created.RefundId }, _mapper.Map<RefundResponseDTO>(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateRefund(int id, RefundRequestDTO refundDto)
        {
            try
            {
                var refund = _mapper.Map<Refund>(refundDto);
                var updated = await _refundRepository.UpdateAsync(id, refund);
                if (updated == null) return NotFound();

                var refreshed = await _refundRepository.GetByIdAsync(id);
                return Ok(_mapper.Map<RefundResponseDTO>(refreshed));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRefund(int id)
        {
            try
            {
                var success = await _refundRepository.DeleteAsync(id);
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

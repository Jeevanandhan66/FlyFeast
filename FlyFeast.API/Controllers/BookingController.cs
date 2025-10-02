using AutoMapper;
using FlyFeast.API.DTOs.Bookings;
using FlyFeast.API.Models;
using FlyFeast.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager,Customer")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingController(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        // -------------------- GET ALL BOOKINGS --------------------
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                var bookings = await _bookingRepository.GetAllAsync();
                return Ok(_mapper.Map<List<BookingResponseDTO>>(bookings));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // -------------------- GET BOOKING BY ID --------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                if (booking == null) return NotFound();
                return Ok(_mapper.Map<BookingResponseDTO>(booking));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // -------------------- GET BOOKINGS BY USER --------------------
        [HttpGet("byuser/{userId}")]
        public async Task<IActionResult> GetBookingsByUser(string userId)
        {
            try
            {
                var bookings = await _bookingRepository.GetByUserIdAsync(userId);
                return Ok(_mapper.Map<List<BookingResponseDTO>>(bookings));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // -------------------- CREATE BOOKING --------------------
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDTO bookingDto)
        {
            try
            {
                if (bookingDto == null || bookingDto.Seats == null || !bookingDto.Seats.Any())
                    return BadRequest("Booking must include at least one seat.");

                var booking = new Booking
                {
                    UserId = bookingDto.UserId,
                    ScheduleId = bookingDto.ScheduleId,
                    Status = BookingStatus.Pending, // always server-controlled
                    CreatedAt = DateTime.UtcNow
                };

                var seatPassengerPairs = bookingDto.Seats
                    .Select(s => (s.SeatId, s.PassengerId))
                    .ToList();

                var createdBooking = await _bookingRepository.AddAsync(booking, seatPassengerPairs);

                var fullBooking = await _bookingRepository.GetByIdAsync(createdBooking.BookingId);
                if (fullBooking == null) return NotFound("Booking not found after creation.");

                return CreatedAtAction(nameof(GetBooking), new { id = fullBooking.BookingId },
                                       _mapper.Map<BookingResponseDTO>(fullBooking));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not create booking: {ex.Message}" });
            }
        }

        // -------------------- UPDATE BOOKING STATUS --------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingRequestDTO bookingDto)
        {
            try
            {
                var booking = _mapper.Map<Booking>(bookingDto);
                var updated = await _bookingRepository.UpdateAsync(id, booking);
                if (updated == null) return NotFound();

                return Ok(_mapper.Map<BookingResponseDTO>(updated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not update booking: {ex.Message}" });
            }
        }

        // -------------------- CANCEL BOOKING --------------------
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Admin,Manager,Customer")]
        public async Task<IActionResult> CancelBooking(int id, [FromBody] BookingCancellationDTO cancellationDto)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                if (booking == null)
                    return NotFound($"Booking with ID {id} not found.");

                if (booking.Status == BookingStatus.Cancelled)
                    return BadRequest("Booking is already cancelled.");

                var cancellation = new BookingCancellation
                {
                    BookingId = booking.BookingId,
                    Reason = cancellationDto.Reason,
                    CancelledAt = DateTime.UtcNow
                };

                await _bookingRepository.AddCancellationAsync(cancellation);

                return Ok(new
                {
                    Message = "Booking cancelled successfully.",
                    cancellation.CancellationId,
                    cancellation.CancelledAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not cancel booking: {ex.Message}" });
            }
        }

        // -------------------- DELETE BOOKING --------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var success = await _bookingRepository.DeleteAsync(id);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Could not delete booking: {ex.Message}" });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonBookingApp.Data;
using SalonBookingApp.DTOs;
using SalonBookingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SalonBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Stylist)
                .Include(b => b.Service)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                DateTime = b.DateTime,
                CustomerName = b.Customer?.FullName ?? string.Empty,
                StylistName = b.Stylist?.Name ?? string.Empty,
                ServiceName = b.Service?.Name ?? string.Empty
            }).ToList();

            return Ok(bookingDtos);
        }

        // GET: api/booking/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Stylist)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            var dto = new BookingDto
            {
                Id = booking.Id,
                DateTime = booking.DateTime,
                CustomerName = booking.Customer?.FullName ?? string.Empty,
                StylistName = booking.Stylist?.Name ?? string.Empty,
                ServiceName = booking.Service?.Name ?? string.Empty
            };

            return Ok(dto);
        }

        // POST: api/booking
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingCreateDto dto)
        {
            var booking = new Booking
            {
                DateTime = dto.DateTime,
                CustomerId = dto.CustomerId,
                StylistId = dto.StylistId,
                ServiceId = dto.ServiceId,
                Notes = dto.Notes ?? string.Empty,
                Status = BookingStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var createdBooking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Stylist)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            if (createdBooking == null)
                return StatusCode(500, "An error occurred while retrieving the booking.");

            var resultDto = new BookingDto
            {
                Id = createdBooking.Id,
                DateTime = createdBooking.DateTime,
                CustomerName = createdBooking.Customer?.FullName ?? string.Empty,
                StylistName = createdBooking.Stylist?.Name ?? string.Empty,
                ServiceName = createdBooking.Service?.Name ?? string.Empty
            };

            return CreatedAtAction(nameof(GetBooking), new { id = resultDto.Id }, resultDto);
        }

        // PUT: api/booking/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingCreateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.DateTime = dto.DateTime;
            booking.CustomerId = dto.CustomerId;
            booking.StylistId = dto.StylistId;
            booking.ServiceId = dto.ServiceId;
            booking.Notes = dto.Notes ?? string.Empty;
            booking.LastModifiedDate = DateTime.UtcNow;

            _context.Entry(booking).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(b => b.Id == id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        // PUT: api/booking/{id}/reschedule
        [HttpPut("{id}/reschedule")]
        public async Task<IActionResult> RescheduleBooking(int id, [FromBody] BookingRescheduleDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.DateTime = dto.NewDateTime;
            booking.Status = BookingStatus.Rescheduled;
            booking.LastModifiedDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(b => b.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // PUT: api/booking/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id, [FromBody] BookingCancelDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = dto.CancellationReason;
            booking.LastModifiedDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(b => b.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // GET: api/booking/{id}/emaillogs
        [HttpGet("{id}/emaillogs")]
        public async Task<ActionResult<IEnumerable<EmailLog>>> GetBookingEmailLogs(int id)
        {
            // Optionally, verify that the booking exists first
            if (!await _context.Bookings.AnyAsync(b => b.Id == id))
                return NotFound();

            var emailLogs = await _context.EmailLogs
                .Where(el => el.BookingId == id)
                .ToListAsync();

            return Ok(emailLogs);
        }
    }
}
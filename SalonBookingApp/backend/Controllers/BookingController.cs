using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonBookingApp.Data;
using SalonBookingApp.DTOs;
using SalonBookingApp.Models;
using SalonBookingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalonBookingApp.Controllers
{
    /// <summary>
    /// API controller for managing bookings.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Constructor for BookingController.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        /// <param name="emailService">The email service used for sending notifications.</param>
        public BookingController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        /// <summary>
        /// Gets all bookings.
        /// </summary>
        /// <returns>A list of booking DTOs.</returns>
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

        /// <summary>
        /// Gets a booking by its ID.
        /// </summary>
        /// <param name="id">The ID of the booking.</param>
        /// <returns>A booking DTO if found; otherwise, 404 Not Found.</returns>
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

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="dto">A DTO containing the necessary booking information.</param>
        /// <returns>The created booking DTO.</returns>
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

        /// <summary>
        /// Updates an existing booking.
        /// </summary>
        /// <param name="id">The ID of the booking to update.</param>
        /// <param name="dto">A DTO containing the updated booking information.</param>
        /// <returns>No content if successful; otherwise, an error status.</returns>
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

        /// <summary>
        /// Reschedules an existing booking.
        /// </summary>
        /// <param name="id">The ID of the booking to reschedule.</param>
        /// <param name="dto">A DTO containing the new appointment date and an optional reason.</param>
        /// <returns>No content if successful; otherwise, an error status.</returns>
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

            // Send reschedule email
            var customerEmail = (await _context.Customers.FindAsync(booking.CustomerId))?.Email;
            if (!string.IsNullOrEmpty(customerEmail))
            {
                await _emailService.SendEmailAsync(
                    customerEmail,
                    "Your appointment has been rescheduled",
                    $"Your appointment has been rescheduled to {booking.DateTime}. {(string.IsNullOrEmpty(dto.Reason) ? "" : "Reason: " + dto.Reason)}"
                );
            }

            return NoContent();
        }

        /// <summary>
        /// Cancels an existing booking.
        /// </summary>
        /// <param name="id">The ID of the booking to cancel.</param>
        /// <param name="dto">A DTO containing the cancellation reason.</param>
        /// <returns>No content if successful; otherwise, an error status.</returns>
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

            // Send cancellation email
            var customerEmail = (await _context.Customers.FindAsync(booking.CustomerId))?.Email;
            if (!string.IsNullOrEmpty(customerEmail))
            {
                await _emailService.SendEmailAsync(
                    customerEmail,
                    "Your appointment has been cancelled",
                    $"Your appointment scheduled for {booking.DateTime} has been cancelled. Reason: {dto.CancellationReason}"
                );
            }

            return NoContent();
        }

        /// <summary>
        /// Retrieves the email log for a specific booking.
        /// </summary>
        /// <param name="id">The ID of the booking for which to retrieve email logs.</param>
        /// <returns>A list of email logs related to the booking.</returns>
        [HttpGet("{id}/emaillogs")]
        public async Task<ActionResult<IEnumerable<EmailLog>>> GetBookingEmailLogs(int id)
        {
            if (!await _context.Bookings.AnyAsync(b => b.Id == id))
                return NotFound();

            var emailLogs = await _context.EmailLogs
                .Where(el => el.BookingId == id)
                .ToListAsync();

            return Ok(emailLogs);
        }
    }
}

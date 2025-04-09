using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonBookingApp.Data;
using SalonBookingApp.DTOs;
using SalonBookingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalonBookingApp.Controllers
{
    /// <summary>
    /// API controller for managing stylists.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class StylistsController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="StylistsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public StylistsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all stylists.
        /// </summary>
        /// <returns>A list of stylist objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stylist>>> GetStylists()
        {
            return await _context.Stylists.ToListAsync();
        }

        /// <summary>
        /// Retrieves a stylist by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the stylist.</param>
        /// <returns>The stylist if found; otherwise, a 404 error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Stylist>> GetStylist(int id)
        {
            var stylist = await _context.Stylists.FindAsync(id);
            if (stylist == null)
                return NotFound();
            return stylist;
        }

        /// <summary>
        /// Creates a new stylist.
        /// </summary>
        /// <param name="dto">The data transfer object containing stylist details.</param>
        /// <returns>The newly created stylist.</returns>
        [HttpPost]
        public async Task<ActionResult<Stylist>> CreateStylist([FromBody] StylistCreateDto dto)
        {
            var stylist = new Stylist
            {
                Name = dto.Name,
                Specialty = dto.Specialty
            };

            _context.Stylists.Add(stylist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStylist), new { id = stylist.Id }, stylist);
        }

        /// <summary>
        /// Updates an existing stylist.
        /// </summary>
        /// <param name="id">The ID of the stylist to update.</param>
        /// <param name="dto">The updated stylist details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStylist(int id, [FromBody] StylistCreateDto dto)
        {
            var stylist = await _context.Stylists.FindAsync(id);
            if (stylist == null)
                return NotFound();

            stylist.Name = dto.Name;
            stylist.Specialty = dto.Specialty;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a stylist.
        /// </summary>
        /// <param name="id">The ID of the stylist to delete.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStylist(int id)
        {
            var stylist = await _context.Stylists.FindAsync(id);
            if (stylist == null)
                return NotFound();

            _context.Stylists.Remove(stylist);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

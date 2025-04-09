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
    /// API controller for managing services.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all services.
        /// </summary>
        /// <returns>A list of service objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        /// <summary>
        /// Retrieves a service by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the service.</param>
        /// <returns>The service if found; otherwise, a 404 error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();
            return service;
        }

        /// <summary>
        /// Creates a new service.
        /// </summary>
        /// <param name="dto">The data transfer object containing service details.</param>
        /// <returns>The newly created service.</returns>
        [HttpPost]
        public async Task<ActionResult<Service>> CreateService([FromBody] ServiceCreateDto dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                Price = dto.Price
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        /// <summary>
        /// Updates an existing service.
        /// </summary>
        /// <param name="id">The ID of the service to update.</param>
        /// <param name="dto">The updated service details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceCreateDto dto)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            service.Name = dto.Name;
            service.Price = dto.Price;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a service.
        /// </summary>
        /// <param name="id">The ID of the service to delete.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

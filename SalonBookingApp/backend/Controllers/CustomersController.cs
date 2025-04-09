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
    /// API controller for managing customers.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <returns>A list of customer objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        /// <summary>
        /// Retrieves a customer by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the customer.</param>
        /// <returns>The customer if found; otherwise, a 404 error.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            return customer;
        }

        /// <summary>
        /// Creates a new customer.
        /// </summary>
        /// <param name="dto">The data transfer object containing customer details.</param>
        /// <returns>The newly created customer.</returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CustomerCreateDto dto)
        {
            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="id">The ID of the customer to update.</param>
        /// <param name="dto">The updated customer details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerCreateDto dto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a customer.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

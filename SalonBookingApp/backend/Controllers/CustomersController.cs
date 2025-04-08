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
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();
            return customer;
        }

        // POST: api/customers
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

        // PUT: api/customers/{id}
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

        // DELETE: api/customers/{id}
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

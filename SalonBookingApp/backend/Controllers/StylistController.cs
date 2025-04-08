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
	public class StylistsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public StylistsController(AppDbContext context)
		{
			_context = context;
		}

		// GET: api/stylists
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Stylist>>> GetStylists()
		{
			return await _context.Stylists.ToListAsync();
		}

		// GET: api/stylists/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Stylist>> GetStylist(int id)
		{
			var stylist = await _context.Stylists.FindAsync(id);
			if (stylist == null)
				return NotFound();
			return stylist;
		}

		// POST: api/stylists
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

		// PUT: api/stylists/{id}
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

		// DELETE: api/stylists/{id}
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

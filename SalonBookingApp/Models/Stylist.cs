using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalonBookingApp.Models
{
    public class Stylist
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
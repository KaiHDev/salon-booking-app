using System;

namespace SalonBookingApp.DTOs
{
    public class BookingCreateDto
    {
        public DateTime DateTime { get; set; }
        public int CustomerId { get; set; }
        public int StylistId { get; set; }
        public int ServiceId { get; set; }
        public string Notes { get; set; }
    }
}
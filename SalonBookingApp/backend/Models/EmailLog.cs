using System;
using System.ComponentModel.DataAnnotations;

namespace SalonBookingApp.Models
{
    public class EmailLog
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        public EmailNotificationType NotificationType { get; set; }
        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public string Message { get; set; } = string.Empty;
    }
}
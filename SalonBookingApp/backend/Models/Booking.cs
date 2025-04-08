using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalonBookingApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }

        public int CustomerId { get; set; }
        public int StylistId { get; set; }
        public int ServiceId { get; set; }

        public Customer Customer { get; set; } = null!;
        public Stylist Stylist { get; set; } = null!;
        public Service Service { get; set; } = null!;

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        public string Notes { get; set; } = string.Empty;
        public string CancellationReason { get; set; } = string.Empty;

        public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
    }
}
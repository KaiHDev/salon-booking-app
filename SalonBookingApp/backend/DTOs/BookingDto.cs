namespace SalonBookingApp.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string StylistName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }
}

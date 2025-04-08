namespace SalonBookingApp.Models
{
    public enum BookingStatus
    {
        Pending,      // Appointment created but not yet confirmed.
        Confirmed,    // Appointment confirmed.
        Rescheduled,  // Appointment has been rescheduled.
        Cancelled,    // Appointment has been cancelled.
        Completed     // Appointment has been fulfilled/completed.
    }

    public enum EmailNotificationType
    {
        AppointmentCreated,
        AppointmentRescheduled,
        AppointmentDeleted,
        AppointmentCompleted
    }
}

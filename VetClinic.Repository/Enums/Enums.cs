namespace VetClinic.Repository.Enums
{
    public enum UserRole
    {
        Customer = 1,
        Doctor = 2,
        Staff = 3,
        Manager = 4,
        Admin = 5
    }

    public enum AppointmentStatus
    {
        Scheduled = 1,
        Confirmed = 2,
        InProgress = 3, 
        Completed = 4,
        Cancelled = 5
    }

    public enum VaccinationStatus
    {
        Due = 1,
        Completed = 2,
        Overdue = 3
    }

    public enum InvoiceStatus
    {
        Pending = 1,
        Paid = 2,
        Cancelled = 3,
        Overdue = 4
    }

    public enum ReminderStatus
    {
        Pending = 1,
        Sent = 2,
        Failed = 3
    }

    public enum ReminderType
    {
        Appointment = 1,
        Vaccination = 2,
        FollowUp = 3,
        Invoice = 4
    }
}

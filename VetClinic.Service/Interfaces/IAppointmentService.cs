using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<bool> AppointmentExistsAsync(int id);

        // Appointment-specific business methods
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetAllAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPetAsync(int petId);
        Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date);
        Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId);
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status, int userId);
        Task<bool> CanUserAccessAppointmentAsync(int userId, int appointmentId, string userRole);
        Task<Appointment> BookAppointmentAsync(int petId, int doctorId, int serviceId, DateTime appointmentTime, string? notes = null);
        Task<bool> CancelAppointmentAsync(int appointmentId, int userId, string userRole);
        Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newAppointmentTime, int userId, string userRole);
        Task<IEnumerable<Appointment>> GetAppointmentsByOwnerAsync(int ownerId);
    }
}
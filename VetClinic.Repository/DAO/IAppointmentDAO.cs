using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IAppointmentDAO : IBaseDAO<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByPetAsync(int petId);
        Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date);
        Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId);
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);
    }
}
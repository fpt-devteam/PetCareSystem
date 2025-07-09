using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IAppointmentDAO _appointmentDAO;
        private readonly IServiceDAO _serviceDAO;

        public AppointmentRepository(IAppointmentDAO appointmentDAO, IServiceDAO serviceDAO)
        {
            _appointmentDAO = appointmentDAO;
            _serviceDAO = serviceDAO;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _appointmentDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _appointmentDAO.GetAllAsync();
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            // Business logic validation
            if (appointment.PetId <= 0)
                throw new ArgumentException("Valid pet ID is required");

            if (appointment.DoctorId <= 0)
                throw new ArgumentException("Valid doctor ID is required");

            if (appointment.ServiceId <= 0)
                throw new ArgumentException("Valid service ID is required");

            if (appointment.AppointmentTime <= DateTime.Now)
                throw new ArgumentException("Appointment time must be in the future");

            // Check if service exists
            var service = await _serviceDAO.GetByIdAsync(appointment.ServiceId);
            if (service == null)
                throw new ArgumentException("Service not found");

            // Check doctor availability
            var isAvailable = await _appointmentDAO.IsDoctorAvailableAsync(
                appointment.DoctorId,
                appointment.AppointmentTime,
                service.DurationMinutes);

            if (!isAvailable)
                throw new ArgumentException("Doctor is not available at the requested time");

            appointment.CreatedDate = DateTime.Now;
            appointment.Status = "Scheduled";
            return await _appointmentDAO.AddAsync(appointment);
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            // Business logic validation
            var existingAppointment = await _appointmentDAO.GetByIdAsync(appointment.Id);
            if (existingAppointment == null)
                throw new ArgumentException("Appointment not found");

            if (appointment.PetId <= 0)
                throw new ArgumentException("Valid pet ID is required");

            if (appointment.DoctorId <= 0)
                throw new ArgumentException("Valid doctor ID is required");

            if (appointment.ServiceId <= 0)
                throw new ArgumentException("Valid service ID is required");

            return await _appointmentDAO.UpdateAsync(appointment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _appointmentDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _appointmentDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date)
        {
            return await _appointmentDAO.GetAppointmentsByDoctorAsync(doctorId, date);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPetAsync(int petId)
        {
            return await _appointmentDAO.GetAppointmentsByPetAsync(petId);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date)
        {
            return await _appointmentDAO.GetAppointmentsForDateAsync(date);
        }

        public async Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date)
        {
            return await _appointmentDAO.GetDailyAgendaAsync(date);
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _appointmentDAO.GetAppointmentWithDetailsAsync(appointmentId);
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes)
        {
            return await _appointmentDAO.IsDoctorAvailableAsync(doctorId, appointmentTime, durationMinutes);
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7)
        {
            return await _appointmentDAO.GetUpcomingAppointmentsAsync(days);
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            return await _appointmentDAO.UpdateAppointmentStatusAsync(appointmentId, status);
        }
    }
}
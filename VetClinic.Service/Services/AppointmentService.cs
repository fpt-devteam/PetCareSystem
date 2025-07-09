using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPetRepository _petRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IPetRepository petRepository,
            IServiceRepository serviceRepository,
            IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _petRepository = petRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _appointmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAsync();
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            return await _appointmentRepository.CreateAsync(appointment);
        }

        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            return await _appointmentRepository.UpdateAsync(appointment);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _appointmentRepository.DeleteAsync(id);
        }

        public async Task<bool> AppointmentExistsAsync(int id)
        {
            return await _appointmentRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date)
        {
            return await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId, date);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPetAsync(int petId)
        {
            return await _appointmentRepository.GetAppointmentsByPetAsync(petId);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date)
        {
            return await _appointmentRepository.GetAppointmentsForDateAsync(date);
        }

        public async Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date)
        {
            return await _appointmentRepository.GetDailyAgendaAsync(date);
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes)
        {
            return await _appointmentRepository.IsDoctorAvailableAsync(doctorId, appointmentTime, durationMinutes);
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7)
        {
            return await _appointmentRepository.GetUpcomingAppointmentsAsync(days);
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status, int userId)
        {
            return await _appointmentRepository.UpdateAppointmentStatusAsync(appointmentId, status);
        }

        public async Task<bool> CanUserAccessAppointmentAsync(int userId, int appointmentId, string userRole)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            // Admins and managers can access all appointments
            if (userRole == "Admin" || userRole == "Manager") return true;

            // Doctors can access appointments assigned to them
            if (userRole == "Doctor" && appointment.DoctorId == userId) return true;

            // Customers can access appointments for their pets
            if (userRole == "Customer")
            {
                var pet = await _petRepository.GetByIdAsync(appointment.PetId);
                return pet != null && pet.OwnerId == userId;
            }

            // Staff can access all appointments
            if (userRole == "Staff") return true;

            return false;
        }

        public async Task<Appointment> BookAppointmentAsync(int petId, int doctorId, int serviceId, DateTime appointmentTime, string? notes = null)
        {
            // Comprehensive business validation
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null)
                throw new ArgumentException("Pet not found");

            var doctor = await _userRepository.GetByIdAsync(doctorId);
            if (doctor == null || doctor.Role != "Doctor")
                throw new ArgumentException("Doctor not found");

            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null || !service.IsActive)
                throw new ArgumentException("Service not found or not active");

            // Check if appointment time is during business hours (8 AM - 6 PM)
            if (appointmentTime.Hour < 8 || appointmentTime.Hour >= 18)
                throw new ArgumentException("Appointments can only be scheduled between 8 AM and 6 PM");

            // Check if appointment is on a weekday
            if (appointmentTime.DayOfWeek == DayOfWeek.Sunday)
                throw new ArgumentException("Appointments cannot be scheduled on Sundays");

            var appointment = new Appointment
            {
                PetId = petId,
                DoctorId = doctorId,
                ServiceId = serviceId,
                AppointmentTime = appointmentTime,
                Notes = notes
            };

            return await _appointmentRepository.CreateAsync(appointment);
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, int userId, string userRole)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            // Check if user can access this appointment
            if (!await CanUserAccessAppointmentAsync(userId, appointmentId, userRole))
                return false;

            // Check if appointment can be cancelled (not in the past and not already completed)
            if (appointment.AppointmentTime <= DateTime.Now || appointment.Status == "Completed")
                return false;

            return await _appointmentRepository.UpdateAppointmentStatusAsync(appointmentId, "Cancelled");
        }

        public async Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newAppointmentTime, int userId, string userRole)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null) return false;

            // Check if user can access this appointment
            if (!await CanUserAccessAppointmentAsync(userId, appointmentId, userRole))
                return false;

            // Validate new appointment time
            if (newAppointmentTime <= DateTime.Now)
                throw new ArgumentException("New appointment time must be in the future");

            // Check if appointment time is during business hours
            if (newAppointmentTime.Hour < 8 || newAppointmentTime.Hour >= 18)
                throw new ArgumentException("Appointments can only be scheduled between 8 AM and 6 PM");

            // Get service to check duration for availability
            var service = await _serviceRepository.GetByIdAsync(appointment.ServiceId);
            if (service == null) return false;

            // Check doctor availability at new time
            var isAvailable = await _appointmentRepository.IsDoctorAvailableAsync(
                appointment.DoctorId, newAppointmentTime, service.DurationMinutes);

            if (!isAvailable)
                throw new ArgumentException("Doctor is not available at the new appointment time");

            // Update appointment
            appointment.AppointmentTime = newAppointmentTime;
            appointment.Status = "Rescheduled";

            await _appointmentRepository.UpdateAsync(appointment);
            return true;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByOwnerAsync(int ownerId)
        {
            var pets = await _petRepository.GetPetsByOwnerIdAsync(ownerId);
            var petIds = pets.Select(p => p.Id).ToList();
            
            var appointments = new List<Appointment>();
            foreach (var petId in petIds)
            {
                var petAppointments = await _appointmentRepository.GetAppointmentsByPetAsync(petId);
                appointments.AddRange(petAppointments);
            }
            
            return appointments.OrderBy(a => a.AppointmentTime);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class AppointmentDAO : BaseDAO<Appointment>, IAppointmentDAO
    {
        public AppointmentDAO(VetClinicDbContext context) : base(context)
        {
        }

        // Override GetAllAsync to include navigation properties
        public override async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Where(a => a.DoctorId == doctorId && a.AppointmentTime.Date == date.Date)
                .Include(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPetAsync(int petId)
        {
            return await _dbSet
                .Where(a => a.PetId == petId)
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(a => a.AppointmentTime.Date == date.Date)
                .Include(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date)
        {
            return await _dbSet
                .Where(a => a.AppointmentTime.Date == date.Date && a.Status != "Cancelled")
                .Include(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .Include(a => a.MedicalRecord)
                .Include(a => a.Invoice)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes)
        {
            var endTime = appointmentTime.AddMinutes(durationMinutes);

            var conflictingAppointments = await _dbSet
                .Where(a => a.DoctorId == doctorId
                    && a.Status != "Cancelled"
                    && ((a.AppointmentTime <= appointmentTime && a.AppointmentTime.AddMinutes(a.Service.DurationMinutes) > appointmentTime)
                        || (a.AppointmentTime < endTime && a.AppointmentTime >= appointmentTime)))
                .Include(a => a.Service)
                .CountAsync();

            return conflictingAppointments == 0;
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int days = 7)
        {
            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(days);

            return await _dbSet
                .Where(a => a.AppointmentTime >= startDate && a.AppointmentTime <= endDate && a.Status == "Scheduled")
                .Include(a => a.Pet)
                .ThenInclude(p => p.Owner)
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                return false;
            }

            appointment.Status = status;
            
            try
            {
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
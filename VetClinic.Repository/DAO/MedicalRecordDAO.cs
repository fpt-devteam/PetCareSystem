using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class MedicalRecordDAO : BaseDAO<MedicalRecord>, IMedicalRecordDAO
    {
        public MedicalRecordDAO(VetClinicDbContext context) : base(context)
        {
        }

        public override async Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Service)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Invoice)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public override async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Service)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Invoice)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetAsync(int petId)
        {
            return await _dbSet
                .Where(m => m.PetId == petId)
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Service)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Invoice)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Where(m => m.DoctorId == doctorId)
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Service)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Invoice)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetMedicalRecordByAppointmentAsync(int appointmentId)
        {
            return await _dbSet
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .FirstOrDefaultAsync(m => m.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(m => m.VisitDate >= startDate && m.VisitDate <= endDate)
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Service)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Invoice)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetPetMedicalHistoryAsync(int petId)
        {
            return await _dbSet
                .Where(m => m.PetId == petId)
                .Include(m => m.Pet)
                .ThenInclude(p => p.Owner)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Service)
                .Include(m => m.Appointment)
                .ThenInclude(a => a.Invoice)
                .OrderByDescending(m => m.VisitDate)
                .ToListAsync();
        }
    }
}

using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly IMedicalRecordDAO _medicalRecordDAO;

        public MedicalRecordRepository(IMedicalRecordDAO medicalRecordDAO)
        {
            _medicalRecordDAO = medicalRecordDAO;
        }

        public async Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return await _medicalRecordDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _medicalRecordDAO.GetAllAsync();
        }

        public async Task<MedicalRecord> CreateAsync(MedicalRecord medicalRecord)
        {
            // Business logic validation
            if (medicalRecord.AppointmentId <= 0)
                throw new ArgumentException("Valid appointment ID is required");

            if (medicalRecord.PetId <= 0)
                throw new ArgumentException("Valid pet ID is required");

            if (medicalRecord.DoctorId <= 0)
                throw new ArgumentException("Valid doctor ID is required");

            if (medicalRecord.VisitDate == default)
                throw new ArgumentException("Visit date is required");

            // Check if medical record already exists for this appointment
            var existing = await _medicalRecordDAO.GetMedicalRecordByAppointmentAsync(medicalRecord.AppointmentId);
            if (existing != null)
                throw new ArgumentException("Medical record already exists for this appointment");

            medicalRecord.CreatedDate = DateTime.Now;
            return await _medicalRecordDAO.AddAsync(medicalRecord);
        }

        public async Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord)
        {
            // Business logic validation
            var existingRecord = await _medicalRecordDAO.GetByIdAsync(medicalRecord.Id);
            if (existingRecord == null)
                throw new ArgumentException("Medical record not found");

            if (medicalRecord.VisitDate == default)
                throw new ArgumentException("Visit date is required");

            return await _medicalRecordDAO.UpdateAsync(medicalRecord);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _medicalRecordDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _medicalRecordDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetAsync(int petId)
        {
            return await _medicalRecordDAO.GetMedicalRecordsByPetAsync(petId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorAsync(int doctorId)
        {
            return await _medicalRecordDAO.GetMedicalRecordsByDoctorAsync(doctorId);
        }

        public async Task<MedicalRecord?> GetMedicalRecordByAppointmentAsync(int appointmentId)
        {
            return await _medicalRecordDAO.GetMedicalRecordByAppointmentAsync(appointmentId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _medicalRecordDAO.GetMedicalRecordsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<MedicalRecord>> GetPetMedicalHistoryAsync(int petId)
        {
            return await _medicalRecordDAO.GetPetMedicalHistoryAsync(petId);
        }
    }
}
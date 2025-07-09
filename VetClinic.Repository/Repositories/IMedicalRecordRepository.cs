using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IMedicalRecordRepository
    {
        Task<MedicalRecord?> GetByIdAsync(int id);
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord> CreateAsync(MedicalRecord medicalRecord);
        Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Medical record-specific methods
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetAsync(int petId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecord?> GetMedicalRecordByAppointmentAsync(int appointmentId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MedicalRecord>> GetPetMedicalHistoryAsync(int petId);
    }
}
using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id);
        Task<IEnumerable<MedicalRecord>> GetAllMedicalRecordsAsync();
        Task<MedicalRecord> CreateMedicalRecordAsync(MedicalRecord medicalRecord);
        Task<MedicalRecord> UpdateMedicalRecordAsync(MedicalRecord medicalRecord);
        Task<bool> DeleteMedicalRecordAsync(int id);
        Task<bool> MedicalRecordExistsAsync(int id);

        // Medical record-specific business methods
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetAsync(int petId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecord?> GetMedicalRecordByAppointmentAsync(int appointmentId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MedicalRecord>> GetPetMedicalHistoryAsync(int petId);
        Task<bool> CanUserAccessMedicalRecordAsync(int userId, int medicalRecordId, string userRole);
        Task<MedicalRecord> CreateMedicalRecordFromAppointmentAsync(int appointmentId, string diagnosis, string treatmentNotes, string? prescription = null);
    }
}
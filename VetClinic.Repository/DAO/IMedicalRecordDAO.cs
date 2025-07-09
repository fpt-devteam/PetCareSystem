using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IMedicalRecordDAO : IBaseDAO<MedicalRecord>
    {
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetAsync(int petId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecord?> GetMedicalRecordByAppointmentAsync(int appointmentId);
        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MedicalRecord>> GetPetMedicalHistoryAsync(int petId);
    }
}
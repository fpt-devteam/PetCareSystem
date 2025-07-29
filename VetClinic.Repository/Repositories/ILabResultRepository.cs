using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface ILabResultRepository
    {
        Task<LabResult?> GetByIdAsync(int id);
        Task<IEnumerable<LabResult>> GetAllAsync();
        Task<LabResult> CreateAsync(LabResult labResult);
        Task<LabResult> UpdateAsync(LabResult labResult);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Lab result-specific business methods
        Task<IEnumerable<LabResult>> GetLabResultsByMedicalRecordAsync(int medicalRecordId);
        Task<IEnumerable<LabResult>> GetLabResultsByUploaderAsync(int uploadedBy);
        Task<IEnumerable<LabResult>> GetActiveLabResultsAsync();
        Task<LabResult?> GetLabResultByFileNameAsync(string fileName, int medicalRecordId);
    }
}

using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface ILabResultDAO : IBaseDAO<LabResult>
    {
        Task<IEnumerable<LabResult>> GetLabResultsByMedicalRecordAsync(int medicalRecordId);
        Task<IEnumerable<LabResult>> GetLabResultsByUploaderAsync(int uploadedBy);
        Task<IEnumerable<LabResult>> GetActiveLabResultsAsync();
        Task<LabResult?> GetLabResultByFileNameAsync(string fileName, int medicalRecordId);
    }
}

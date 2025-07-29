using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface ILabResultService
    {
        Task<LabResult?> GetLabResultByIdAsync(int id);
        Task<IEnumerable<LabResult>> GetAllLabResultsAsync();
        Task<LabResult> CreateLabResultAsync(LabResult labResult);
        Task<LabResult> UpdateLabResultAsync(LabResult labResult);
        Task<bool> LabResultExistsAsync(int id);

        // Lab result-specific business methods
        Task<IEnumerable<LabResult>> GetLabResultsByMedicalRecordAsync(int medicalRecordId);
        Task<IEnumerable<LabResult>> GetLabResultsByUploaderAsync(int uploadedBy);
        Task<IEnumerable<LabResult>> GetActiveLabResultsAsync();
        Task<LabResult?> GetLabResultByFileNameAsync(string fileName, int medicalRecordId);

        // File upload methods
        Task<LabResult> UploadLabResultAsync(int medicalRecordId, int uploadedBy, string fileName, string fileType, long fileSize, string contentType, byte[] fileContent, string? description = null, string? labName = null, DateTime? testDate = null);
        Task<bool> DeleteLabResultAsync(int labResultId);
        Task<byte[]?> GetLabResultFileAsync(int labResultId);
    }
}

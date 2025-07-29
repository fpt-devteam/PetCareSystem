using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class LabResultService : ILabResultService
    {
        private readonly ILabResultRepository _labResultRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public LabResultService(ILabResultRepository labResultRepository, IMedicalRecordRepository medicalRecordRepository)
        {
            _labResultRepository = labResultRepository;
            _medicalRecordRepository = medicalRecordRepository;
        }

        public async Task<LabResult?> GetLabResultByIdAsync(int id)
        {
            return await _labResultRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<LabResult>> GetAllLabResultsAsync()
        {
            return await _labResultRepository.GetAllAsync();
        }

        public async Task<LabResult> CreateLabResultAsync(LabResult labResult)
        {
            return await _labResultRepository.CreateAsync(labResult);
        }

        public async Task<LabResult> UpdateLabResultAsync(LabResult labResult)
        {
            return await _labResultRepository.UpdateAsync(labResult);
        }

        public async Task<bool> DeleteLabResultAsync(int id)
        {
            return await _labResultRepository.DeleteAsync(id);
        }

        public async Task<bool> LabResultExistsAsync(int id)
        {
            return await _labResultRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<LabResult>> GetLabResultsByMedicalRecordAsync(int medicalRecordId)
        {
            return await _labResultRepository.GetLabResultsByMedicalRecordAsync(medicalRecordId);
        }

        public async Task<IEnumerable<LabResult>> GetLabResultsByUploaderAsync(int uploadedBy)
        {
            return await _labResultRepository.GetLabResultsByUploaderAsync(uploadedBy);
        }

        public async Task<IEnumerable<LabResult>> GetActiveLabResultsAsync()
        {
            return await _labResultRepository.GetActiveLabResultsAsync();
        }

        public async Task<LabResult?> GetLabResultByFileNameAsync(string fileName, int medicalRecordId)
        {
            return await _labResultRepository.GetLabResultByFileNameAsync(fileName, medicalRecordId);
        }

        public async Task<LabResult> UploadLabResultAsync(int medicalRecordId, int uploadedBy, string fileName, string fileType, long fileSize, string contentType, byte[] fileContent, string? description = null, string? labName = null, DateTime? testDate = null)
        {
            // Validate medical record exists
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(medicalRecordId);
            if (medicalRecord == null)
                throw new ArgumentException("Medical record not found");

            // Validate file size (max 10MB)
            if (fileSize > 10 * 1024 * 1024)
                throw new ArgumentException("File size exceeds maximum allowed size of 10MB");

            var labResult = new LabResult
            {
                MedicalRecordId = medicalRecordId,
                FileName = fileName,
                FileType = fileType,
                FileSize = fileSize,
                FilePath = string.Empty, // No longer needed since we store in DB
                ContentType = contentType,
                FileContent = fileContent,
                Description = description,
                LabName = labName,
                TestDate = testDate,
                UploadedBy = uploadedBy,
                UploadDate = DateTime.Now,
                IsActive = true
            };

            return await CreateLabResultAsync(labResult);
        }

        public async Task<byte[]?> GetLabResultFileAsync(int labResultId)
        {
            var labResult = await GetLabResultByIdAsync(labResultId);
            if (labResult == null || !labResult.IsActive) return null;

            return labResult.FileContent;
        }
    }
}

using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class LabResultRepository : ILabResultRepository
    {
        private readonly ILabResultDAO _labResultDAO;

        public LabResultRepository(ILabResultDAO labResultDAO)
        {
            _labResultDAO = labResultDAO;
        }

        public async Task<LabResult?> GetByIdAsync(int id)
        {
            return await _labResultDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<LabResult>> GetAllAsync()
        {
            return await _labResultDAO.GetAllAsync();
        }

        public async Task<LabResult> CreateAsync(LabResult labResult)
        {
            // Business validation
            if (labResult.MedicalRecordId <= 0)
                throw new ArgumentException("Valid medical record ID is required");

            if (string.IsNullOrWhiteSpace(labResult.FileName))
                throw new ArgumentException("File name is required");

            if (string.IsNullOrWhiteSpace(labResult.FileType))
                throw new ArgumentException("File type is required");



            if (labResult.UploadedBy <= 0)
                throw new ArgumentException("Valid uploader ID is required");

            labResult.UploadDate = DateTime.Now;
            labResult.IsActive = true;
            return await _labResultDAO.AddAsync(labResult);
        }

        public async Task<LabResult> UpdateAsync(LabResult labResult)
        {
            return await _labResultDAO.UpdateAsync(labResult);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _labResultDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _labResultDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<LabResult>> GetLabResultsByMedicalRecordAsync(int medicalRecordId)
        {
            return await _labResultDAO.GetLabResultsByMedicalRecordAsync(medicalRecordId);
        }

        public async Task<IEnumerable<LabResult>> GetLabResultsByUploaderAsync(int uploadedBy)
        {
            return await _labResultDAO.GetLabResultsByUploaderAsync(uploadedBy);
        }

        public async Task<IEnumerable<LabResult>> GetActiveLabResultsAsync()
        {
            return await _labResultDAO.GetActiveLabResultsAsync();
        }

        public async Task<LabResult?> GetLabResultByFileNameAsync(string fileName, int medicalRecordId)
        {
            return await _labResultDAO.GetLabResultByFileNameAsync(fileName, medicalRecordId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class LabResultDAO : BaseDAO<LabResult>, ILabResultDAO
    {
        public LabResultDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LabResult>> GetLabResultsByMedicalRecordAsync(int medicalRecordId)
        {
            return await _dbSet
                .Where(l => l.MedicalRecordId == medicalRecordId && l.IsActive)
                .Include(l => l.MedicalRecord)
                .Include(l => l.UploadedByUser)
                .OrderByDescending(l => l.UploadDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabResult>> GetLabResultsByUploaderAsync(int uploadedBy)
        {
            return await _dbSet
                .Where(l => l.UploadedBy == uploadedBy && l.IsActive)
                .Include(l => l.MedicalRecord)
                .ThenInclude(m => m.Pet)
                .Include(l => l.UploadedByUser)
                .OrderByDescending(l => l.UploadDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabResult>> GetActiveLabResultsAsync()
        {
            return await _dbSet
                .Where(l => l.IsActive)
                .Include(l => l.MedicalRecord)
                .ThenInclude(m => m.Pet)
                .Include(l => l.UploadedByUser)
                .OrderByDescending(l => l.UploadDate)
                .ToListAsync();
        }

        public async Task<LabResult?> GetLabResultByFileNameAsync(string fileName, int medicalRecordId)
        {
            return await _dbSet
                .Where(l => l.FileName == fileName && l.MedicalRecordId == medicalRecordId && l.IsActive)
                .Include(l => l.MedicalRecord)
                .Include(l => l.UploadedByUser)
                .FirstOrDefaultAsync();
        }
    }
}

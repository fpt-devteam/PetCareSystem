using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<AuditLog?> GetAuditLogByIdAsync(int id)
        {
            return await _auditLogRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync()
        {
            return await _auditLogRepository.GetAllAsync();
        }

        public async Task<AuditLog> CreateAuditLogAsync(AuditLog auditLog)
        {
            return await _auditLogRepository.CreateAsync(auditLog);
        }

        public async Task<bool> DeleteAuditLogAsync(int id)
        {
            return await _auditLogRepository.DeleteAsync(id);
        }

        public async Task<bool> AuditLogExistsAsync(int id)
        {
            return await _auditLogRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId)
        {
            return await _auditLogRepository.GetAuditLogsByUserAsync(userId);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityType, int entityId)
        {
            return await _auditLogRepository.GetAuditLogsByEntityAsync(entityType, entityId);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _auditLogRepository.GetAuditLogsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action)
        {
            return await _auditLogRepository.GetAuditLogsByActionAsync(action);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserRoleAsync(string userRole)
        {
            return await _auditLogRepository.GetAuditLogsByUserRoleAsync(userRole);
        }

        public async Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100)
        {
            return await _auditLogRepository.GetRecentAuditLogsAsync(count);
        }

        public async Task<AuditLog> LogEMRAccessAsync(int userId, string userRole, string entityType, int? entityId, string action, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            return await _auditLogRepository.LogAccessAsync(userId, userRole, entityType, entityId, action, description, ipAddress, userAgent);
        }

        public async Task<AuditLog> LogMedicalRecordViewAsync(int userId, string userRole, int medicalRecordId, string? ipAddress = null, string? userAgent = null)
        {
            return await _auditLogRepository.LogAccessAsync(
                userId,
                userRole,
                "MedicalRecord",
                medicalRecordId,
                "VIEW",
                $"User viewed medical record #{medicalRecordId}",
                ipAddress,
                userAgent);
        }

        public async Task<AuditLog> LogMedicalRecordCreateAsync(int userId, string userRole, int medicalRecordId, string? ipAddress = null, string? userAgent = null)
        {
            return await _auditLogRepository.LogAccessAsync(
                userId,
                userRole,
                "MedicalRecord",
                medicalRecordId,
                "CREATE",
                $"User created medical record #{medicalRecordId}",
                ipAddress,
                userAgent);
        }

        public async Task<AuditLog> LogMedicalRecordUpdateAsync(int userId, string userRole, int medicalRecordId, string? ipAddress = null, string? userAgent = null)
        {
            return await _auditLogRepository.LogAccessAsync(
                userId,
                userRole,
                "MedicalRecord",
                medicalRecordId,
                "UPDATE",
                $"User updated medical record #{medicalRecordId}",
                ipAddress,
                userAgent);
        }

        public async Task<AuditLog> LogLabResultUploadAsync(int userId, string userRole, int labResultId, int medicalRecordId, string? ipAddress = null, string? userAgent = null)
        {
            return await _auditLogRepository.LogAccessAsync(
                userId,
                userRole,
                "LabResult",
                labResultId,
                "UPLOAD",
                $"User uploaded lab result #{labResultId} for medical record #{medicalRecordId}",
                ipAddress,
                userAgent);
        }
    }
}

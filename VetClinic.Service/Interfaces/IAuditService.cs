using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IAuditService
    {
        Task<AuditLog?> GetAuditLogByIdAsync(int id);
        Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync();
        Task<AuditLog> CreateAuditLogAsync(AuditLog auditLog);
        Task<bool> DeleteAuditLogAsync(int id);
        Task<bool> AuditLogExistsAsync(int id);

        // Audit log-specific business methods
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action);
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserRoleAsync(string userRole);
        Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100);

        // EMR Access Logging
        Task<AuditLog> LogEMRAccessAsync(int userId, string userRole, string entityType, int? entityId, string action, string? description = null, string? ipAddress = null, string? userAgent = null);
        Task<AuditLog> LogMedicalRecordViewAsync(int userId, string userRole, int medicalRecordId, string? ipAddress = null, string? userAgent = null);
        Task<AuditLog> LogMedicalRecordCreateAsync(int userId, string userRole, int medicalRecordId, string? ipAddress = null, string? userAgent = null);
        Task<AuditLog> LogMedicalRecordUpdateAsync(int userId, string userRole, int medicalRecordId, string? ipAddress = null, string? userAgent = null);
        Task<AuditLog> LogLabResultUploadAsync(int userId, string userRole, int labResultId, int medicalRecordId, string? ipAddress = null, string? userAgent = null);
    }
}

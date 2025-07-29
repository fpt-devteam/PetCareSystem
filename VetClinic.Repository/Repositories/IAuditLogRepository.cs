using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IAuditLogRepository
    {
        Task<AuditLog?> GetByIdAsync(int id);
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<AuditLog> CreateAsync(AuditLog auditLog);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Audit log-specific business methods
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action);
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserRoleAsync(string userRole);
        Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100);
        Task<AuditLog> LogAccessAsync(int userId, string userRole, string entityType, int? entityId, string action, string? description = null, string? ipAddress = null, string? userAgent = null);
    }
}

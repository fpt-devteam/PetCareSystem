using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IAuditLogDAO : IBaseDAO<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action);
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserRoleAsync(string userRole);
        Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100);
    }
}

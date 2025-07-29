using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IAuditLogDAO _auditLogDAO;

        public AuditLogRepository(IAuditLogDAO auditLogDAO)
        {
            _auditLogDAO = auditLogDAO;
        }

        public async Task<AuditLog?> GetByIdAsync(int id)
        {
            return await _auditLogDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _auditLogDAO.GetAllAsync();
        }

        public async Task<AuditLog> CreateAsync(AuditLog auditLog)
        {
            // Business validation
            if (auditLog.UserId <= 0)
                throw new ArgumentException("Valid user ID is required");

            if (string.IsNullOrWhiteSpace(auditLog.Action))
                throw new ArgumentException("Action is required");

            if (string.IsNullOrWhiteSpace(auditLog.EntityType))
                throw new ArgumentException("Entity type is required");

            auditLog.Timestamp = DateTime.Now;
            return await _auditLogDAO.AddAsync(auditLog);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _auditLogDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _auditLogDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(int userId)
        {
            return await _auditLogDAO.GetAuditLogsByUserAsync(userId);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityType, int entityId)
        {
            return await _auditLogDAO.GetAuditLogsByEntityAsync(entityType, entityId);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _auditLogDAO.GetAuditLogsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action)
        {
            return await _auditLogDAO.GetAuditLogsByActionAsync(action);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserRoleAsync(string userRole)
        {
            return await _auditLogDAO.GetAuditLogsByUserRoleAsync(userRole);
        }

        public async Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100)
        {
            return await _auditLogDAO.GetRecentAuditLogsAsync(count);
        }

        public async Task<AuditLog> LogAccessAsync(int userId, string userRole, string entityType, int? entityId, string action, string? description = null, string? ipAddress = null, string? userAgent = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserRole = userRole,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                Description = description,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.Now
            };

            return await CreateAsync(auditLog);
        }
    }
}

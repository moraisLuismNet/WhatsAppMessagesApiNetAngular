using WhatsAppMessagesApiNet.Domain.Entities;

namespace WhatsAppMessagesApiNet.Domain.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId);
    Task<IEnumerable<AuditLog>> GetByUserAsync(string changedBy);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to);
}

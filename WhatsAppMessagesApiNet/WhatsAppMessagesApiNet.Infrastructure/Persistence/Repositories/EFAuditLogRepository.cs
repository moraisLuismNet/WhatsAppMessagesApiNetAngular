using Microsoft.EntityFrameworkCore;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.EF;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class EFAuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public EFAuditLogRepository(AppDbContext context) => _context = context;

    public async Task<AuditLog?> GetByIdAsync(Guid id) => await _context.AuditLogs.FindAsync(id);
    public async Task<IEnumerable<AuditLog>> GetAllAsync() => await _context.AuditLogs.OrderByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<IEnumerable<AuditLog>> FindAsync(System.Linq.Expressions.Expression<Func<AuditLog, bool>> predicate)
        => await _context.AuditLogs.Where(predicate).OrderByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<AuditLog> AddAsync(AuditLog entity) { _context.AuditLogs.Add(entity); return await Task.FromResult(entity); }
    public Task UpdateAsync(AuditLog entity) { _context.AuditLogs.Update(entity); return Task.CompletedTask; }
    public Task DeleteAsync(AuditLog entity) { _context.AuditLogs.Remove(entity); return Task.CompletedTask; }
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId)
        => await _context.AuditLogs.Where(a => a.EntityName == entityName && a.EntityId == entityId).OrderByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<IEnumerable<AuditLog>> GetByUserAsync(string changedBy)
        => await _context.AuditLogs.Where(a => a.ChangedBy == changedBy).OrderByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _context.AuditLogs.Where(a => a.ChangedAt >= from && a.ChangedAt <= to).OrderByDescending(a => a.ChangedAt).ToListAsync();
}

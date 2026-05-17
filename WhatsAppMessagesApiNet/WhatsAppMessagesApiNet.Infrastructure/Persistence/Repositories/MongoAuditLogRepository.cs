using MongoDB.Driver;
using System.Linq.Expressions;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class MongoAuditLogRepository : IAuditLogRepository
{
    private readonly MongoDbContext _context;

    public MongoAuditLogRepository(MongoDbContext context) => _context = context;

    public async Task<AuditLog?> GetByIdAsync(Guid id) => await _context.AuditLogs.Find(a => a.Id == id).FirstOrDefaultAsync();
    public async Task<IEnumerable<AuditLog>> GetAllAsync() => await _context.AuditLogs.Find(_ => true).SortByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<IEnumerable<AuditLog>> FindAsync(Expression<Func<AuditLog, bool>> predicate) => await _context.AuditLogs.Find(predicate).SortByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<AuditLog> AddAsync(AuditLog entity) { await _context.AuditLogs.InsertOneAsync(entity); return entity; }
    public async Task UpdateAsync(AuditLog entity) => await _context.AuditLogs.ReplaceOneAsync(a => a.Id == entity.Id, entity);
    public async Task DeleteAsync(AuditLog entity) => await _context.AuditLogs.DeleteOneAsync(a => a.Id == entity.Id);
    public async Task<int> SaveChangesAsync() => await Task.FromResult(1);
    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId)
        => await _context.AuditLogs.Find(a => a.EntityName == entityName && a.EntityId == entityId).SortByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<IEnumerable<AuditLog>> GetByUserAsync(string changedBy)
        => await _context.AuditLogs.Find(a => a.ChangedBy == changedBy).SortByDescending(a => a.ChangedAt).ToListAsync();
    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _context.AuditLogs.Find(a => a.ChangedAt >= from && a.ChangedAt <= to).SortByDescending(a => a.ChangedAt).ToListAsync();
}

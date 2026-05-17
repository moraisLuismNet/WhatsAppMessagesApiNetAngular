using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class MongoUnitOfWork : IUnitOfWork
{
    private readonly MongoDbContext _context;
    private IUserRepository? _users;
    private IMessageRepository? _messages;
    private IAuditLogRepository? _auditLogs;

    public MongoUnitOfWork(MongoDbContext context) => _context = context;

    public IUserRepository Users => _users ??= new MongoUserRepository(_context);
    public IMessageRepository Messages => _messages ??= new MongoMessageRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogs ??= new MongoAuditLogRepository(_context);

    public async Task<int> CompleteAsync() => await Task.FromResult(1);
    public void Dispose() { }
}

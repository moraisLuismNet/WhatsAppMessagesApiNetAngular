using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.EF;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IUserRepository? _users;
    private IMessageRepository? _messages;
    private IAuditLogRepository? _auditLogs;

    public EFUnitOfWork(AppDbContext context) => _context = context;

    public IUserRepository Users => _users ??= new EFUserRepository(_context);
    public IMessageRepository Messages => _messages ??= new EFMessageRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogs ??= new EFAuditLogRepository(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}

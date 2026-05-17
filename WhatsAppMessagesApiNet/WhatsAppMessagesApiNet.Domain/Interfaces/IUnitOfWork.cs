namespace WhatsAppMessagesApiNet.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IMessageRepository Messages { get; }
    IAuditLogRepository AuditLogs { get; }
    Task<int> CompleteAsync();
}

using WhatsAppMessagesApiNet.Domain.Entities;

namespace WhatsAppMessagesApiNet.Domain.Interfaces;

public interface IMessageRepository : IRepository<Message, int>
{
    Task<IEnumerable<Message>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Message>> GetByStatusAsync(Domain.Enums.MessageStatus status);
    Task<int> GetPendingCountAsync();
}

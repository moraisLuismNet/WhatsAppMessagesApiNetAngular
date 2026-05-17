using WhatsAppMessagesApiNet.Domain.Entities;

namespace WhatsAppMessagesApiNet.Domain.Interfaces;

public interface IUserRepository : IRepository<User, string>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}

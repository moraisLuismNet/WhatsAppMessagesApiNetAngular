using Microsoft.EntityFrameworkCore;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.EF;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class EFMessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public EFMessageRepository(AppDbContext context) => _context = context;

    public async Task<Message?> GetByIdAsync(int id) => await _context.Messages.Include(m => m.User).FirstOrDefaultAsync(m => m.Id == id);
    public async Task<IEnumerable<Message>> GetAllAsync() => await _context.Messages.Include(m => m.User).ToListAsync();
    public async Task<IEnumerable<Message>> FindAsync(System.Linq.Expressions.Expression<Func<Message, bool>> predicate)
        => await _context.Messages.Include(m => m.User).Where(predicate).ToListAsync();
    public async Task<Message> AddAsync(Message entity) { _context.Messages.Add(entity); return await Task.FromResult(entity); }
    public Task UpdateAsync(Message entity) { _context.Messages.Update(entity); return Task.CompletedTask; }
    public Task DeleteAsync(Message entity) { _context.Messages.Remove(entity); return Task.CompletedTask; }
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task<IEnumerable<Message>> GetByUserIdAsync(string userId) => await _context.Messages.Include(m => m.User).Where(m => m.UserId == userId).ToListAsync();
    public async Task<IEnumerable<Message>> GetByStatusAsync(MessageStatus status) => await _context.Messages.Include(m => m.User).Where(m => m.Status == status).ToListAsync();
    public async Task<int> GetPendingCountAsync() => await _context.Messages.CountAsync(m => m.Status == MessageStatus.Pending);
}

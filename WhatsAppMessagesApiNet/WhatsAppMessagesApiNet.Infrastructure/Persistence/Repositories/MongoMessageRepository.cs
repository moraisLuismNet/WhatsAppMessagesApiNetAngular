using MongoDB.Driver;
using System.Linq.Expressions;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class MongoMessageRepository : IMessageRepository
{
    private readonly MongoDbContext _context;

    public MongoMessageRepository(MongoDbContext context) => _context = context;

    public async Task<Message?> GetByIdAsync(int id) => await _context.Messages.Find(m => m.Id == id).FirstOrDefaultAsync();
    public async Task<IEnumerable<Message>> GetAllAsync() => await _context.Messages.Find(_ => true).ToListAsync();
    public async Task<IEnumerable<Message>> FindAsync(Expression<Func<Message, bool>> predicate) => await _context.Messages.Find(predicate).ToListAsync();
    public async Task<Message> AddAsync(Message entity) { await _context.Messages.InsertOneAsync(entity); return entity; }
    public async Task UpdateAsync(Message entity) => await _context.Messages.ReplaceOneAsync(m => m.Id == entity.Id, entity);
    public async Task DeleteAsync(Message entity) => await _context.Messages.DeleteOneAsync(m => m.Id == entity.Id);
    public async Task<int> SaveChangesAsync() => await Task.FromResult(1);
    public async Task<IEnumerable<Message>> GetByUserIdAsync(string userId) => await _context.Messages.Find(m => m.UserId == userId).ToListAsync();
    public async Task<IEnumerable<Message>> GetByStatusAsync(MessageStatus status) => await _context.Messages.Find(m => m.Status == status).ToListAsync();
    public async Task<int> GetPendingCountAsync() => (int)await _context.Messages.CountDocumentsAsync(m => m.Status == MessageStatus.Pending);
}

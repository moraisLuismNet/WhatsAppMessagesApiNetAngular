using MongoDB.Driver;
using System.Linq.Expressions;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public MongoUserRepository(MongoDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(string id) => await _context.Users.Find(u => u.Email == id).FirstOrDefaultAsync();
    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.Find(_ => true).ToListAsync();
    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate) => await _context.Users.Find(predicate).ToListAsync();
    public async Task<User> AddAsync(User entity) { await _context.Users.InsertOneAsync(entity); return entity; }
    public async Task UpdateAsync(User entity) => await _context.Users.ReplaceOneAsync(u => u.Email == entity.Email, entity);
    public async Task DeleteAsync(User entity) => await _context.Users.DeleteOneAsync(u => u.Email == entity.Email);
    public async Task<int> SaveChangesAsync() => await Task.FromResult(1);
    public async Task<User?> GetByEmailAsync(string email) => await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
    public async Task<bool> IsEmailUniqueAsync(string email) => !await _context.Users.Find(u => u.Email == email).AnyAsync();
}

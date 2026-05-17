using Microsoft.EntityFrameworkCore;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.EF;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;

public class EFUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EFUserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(string id) => await _context.Users.FindAsync(id);
    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();
    public async Task<IEnumerable<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
        => await _context.Users.Where(predicate).ToListAsync();
    public async Task<User> AddAsync(User entity) { _context.Users.Add(entity); return await Task.FromResult(entity); }
    public Task UpdateAsync(User entity) { _context.Users.Update(entity); return Task.CompletedTask; }
    public Task DeleteAsync(User entity) { _context.Users.Remove(entity); return Task.CompletedTask; }
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task<User?> GetByEmailAsync(string email) => await _context.Users.FindAsync(email);
    public async Task<bool> IsEmailUniqueAsync(string email) => !await _context.Users.AnyAsync(u => u.Email == email);
}

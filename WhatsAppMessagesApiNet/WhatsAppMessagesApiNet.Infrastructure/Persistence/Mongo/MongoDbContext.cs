using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using WhatsAppMessagesApiNet.Domain.Entities;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
        var databaseName = configuration["MongoDbSettings:DatabaseName"] ?? "WhatsAppMessagesApiNet";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        EnsureIndexes();
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
    public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("AuditLogs");

    private void EnsureIndexes()
    {
        Users.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.PhoneNumber))
        });

        Messages.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Ascending(m => m.UserId)),
            new CreateIndexModel<Message>(Builders<Message>.IndexKeys.Ascending(m => m.Status))
        });

        AuditLogs.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<AuditLog>(Builders<AuditLog>.IndexKeys.Ascending(a => a.EntityName)),
            new CreateIndexModel<AuditLog>(Builders<AuditLog>.IndexKeys.Ascending(a => a.EntityId)),
            new CreateIndexModel<AuditLog>(Builders<AuditLog>.IndexKeys.Ascending(a => a.ChangedAt))
        });
    }
}

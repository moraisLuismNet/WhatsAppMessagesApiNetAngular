using Microsoft.EntityFrameworkCore;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.EF.Seed;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Email = "luis@mail.com",
            Name = "Luis",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = UserRole.Admin,
            IsActive = true
        });
    }
}

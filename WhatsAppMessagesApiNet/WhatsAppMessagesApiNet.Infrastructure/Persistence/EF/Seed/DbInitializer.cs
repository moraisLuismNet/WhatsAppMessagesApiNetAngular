using Microsoft.EntityFrameworkCore;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.EF.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(IUnitOfWork unitOfWork)
    {
        var adminEmail = "luis@mail.com";
        var existingAdmin = await unitOfWork.Users.GetByEmailAsync(adminEmail);

        if (existingAdmin != null)
            return;

        await unitOfWork.Users.AddAsync(new User
        {
            Name = "Luis",
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = UserRole.Admin,
            IsActive = true
        });

        await unitOfWork.CompleteAsync();
    }
}

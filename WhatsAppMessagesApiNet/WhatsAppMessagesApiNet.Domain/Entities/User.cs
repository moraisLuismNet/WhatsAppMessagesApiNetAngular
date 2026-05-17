using WhatsAppMessagesApiNet.Domain.Enums;

namespace WhatsAppMessagesApiNet.Domain.Entities;

public class User
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Operator;
    public bool IsActive { get; set; } = true;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

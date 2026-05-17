using WhatsAppMessagesApiNet.Domain.Common;
using WhatsAppMessagesApiNet.Domain.Enums;

namespace WhatsAppMessagesApiNet.Domain.Entities;

public class Message : BaseEntity<int>
{
    public string UserId { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public MessageStatus Status { get; set; } = MessageStatus.Pending;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public User? User { get; set; }
}

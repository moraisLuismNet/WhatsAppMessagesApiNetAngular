namespace WhatsAppMessagesApiNet.Domain.Common;

public abstract class BaseEntity<TId> : IAuditableEntity
{
    public TId Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}
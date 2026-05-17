namespace WhatsAppMessagesApiNet.Domain.Interfaces;

public class WhatsAppMessageResult
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IWhatsAppProvider
{
    string Name { get; }
    Task<WhatsAppMessageResult> SendTextAsync(string to, string text);
}

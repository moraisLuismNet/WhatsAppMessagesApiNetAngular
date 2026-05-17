namespace WhatsAppMessagesApiNet.Application.DTOs.Message;

public class UpdateMessageStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

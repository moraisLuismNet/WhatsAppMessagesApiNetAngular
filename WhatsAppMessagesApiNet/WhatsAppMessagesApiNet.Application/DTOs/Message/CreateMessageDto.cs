namespace WhatsAppMessagesApiNet.Application.DTOs.Message;

public class CreateMessageDto
{
    public string To { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

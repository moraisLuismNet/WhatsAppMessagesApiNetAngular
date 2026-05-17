namespace WhatsAppMessagesApiNet.Application.DTOs.User;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "Operator";
    public bool IsActive { get; set; } = true;
}

using WhatsAppMessagesApiNet.Application.DTOs.Auth;

namespace WhatsAppMessagesApiNet.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
}

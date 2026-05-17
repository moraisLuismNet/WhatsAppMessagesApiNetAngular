using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WhatsAppMessagesApiNet.Application.DTOs.Auth;
using WhatsAppMessagesApiNet.Application.Interfaces;

namespace WhatsAppMessagesApiNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "After registering, you must activate WhatsApp to receive messages:\n" +
        "\n**1.** Open WhatsApp on your phone  \n" +
        "**2.** Scan this QR code:\n\n" +
        "![QR](https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=https://wa.me/14155238886)\n\n" +
        "**3.** Send the activation code from Twilio Console (`join XXXXX`) to **+1 415 523 8886**  \n" +
        "**4.** You'll now receive messages sent via this API"
    )]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }
}

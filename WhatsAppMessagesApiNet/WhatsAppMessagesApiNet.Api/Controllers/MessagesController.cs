using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppMessagesApiNet.Application.DTOs.Message;
using WhatsAppMessagesApiNet.Application.DTOs.Common;
using WhatsAppMessagesApiNet.Application.Interfaces;

namespace WhatsAppMessagesApiNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService) => _messageService = messageService;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PagedResultDto<MessageDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _messageService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), 200)]
    public async Task<IActionResult> GetMyMessages()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _messageService.GetByRecipientAsync(userEmail);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetById(int id)
    {
        var msg = await _messageService.GetByIdAsync(id);
        if (msg == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        if (msg.UserId != userId && !isAdmin)
            return Forbid();

        return Ok(msg);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MessageDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateMessageDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _messageService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}/status")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateMessageStatusDto dto)
    {
        var result = await _messageService.UpdateStatusAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await _messageService.DeleteAsync(id);
        return NoContent();
    }
}

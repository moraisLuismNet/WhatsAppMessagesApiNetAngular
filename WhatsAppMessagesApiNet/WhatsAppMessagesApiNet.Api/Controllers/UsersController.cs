using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppMessagesApiNet.Application.DTOs.User;
using WhatsAppMessagesApiNet.Application.DTOs.Common;
using WhatsAppMessagesApiNet.Application.Interfaces;

namespace WhatsAppMessagesApiNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<UserDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{email}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var result = await _userService.GetByEmailAsync(email);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetByEmail), new { email = result.Email }, result);
    }

    [HttpPut("{email}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string email, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(email, dto);
        return Ok(result);
    }

    [HttpDelete("{email}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string email)
    {
        await _userService.DeleteAsync(email);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Interfaces;

namespace WhatsAppMessagesApiNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuditLogsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditLog>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _unitOfWork.AuditLogs.GetAllAsync();
        return Ok(logs);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AuditLog), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var log = await _unitOfWork.AuditLogs.GetByIdAsync(id);
        if (log == null) return NotFound();
        return Ok(log);
    }

    [HttpGet("entity/{entityName}/{entityId}")]
    [ProducesResponseType(typeof(IEnumerable<AuditLog>), 200)]
    public async Task<IActionResult> GetByEntity(string entityName, string entityId)
    {
        var logs = await _unitOfWork.AuditLogs.GetByEntityAsync(entityName, entityId);
        return Ok(logs);
    }
}

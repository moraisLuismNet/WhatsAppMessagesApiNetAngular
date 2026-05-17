using WhatsAppMessagesApiNet.Application.DTOs.Message;
using WhatsAppMessagesApiNet.Application.DTOs.Common;

namespace WhatsAppMessagesApiNet.Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto?> GetByIdAsync(int id);
    Task<IEnumerable<MessageDto>> GetAllAsync();
    Task<MessageDto> CreateAsync(CreateMessageDto dto, string userId);
    Task<MessageDto> UpdateStatusAsync(int id, UpdateMessageStatusDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<MessageDto>> GetByUserIdAsync(string userId);
    Task<IEnumerable<MessageDto>> GetByRecipientAsync(string userEmail);
    Task<PagedResultDto<MessageDto>> GetPagedAsync(int page, int pageSize);
}

using WhatsAppMessagesApiNet.Application.DTOs.User;
using WhatsAppMessagesApiNet.Application.DTOs.Common;

namespace WhatsAppMessagesApiNet.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(string email, UpdateUserDto dto);
    Task DeleteAsync(string email);
    Task<PagedResultDto<UserDto>> GetPagedAsync(int page, int pageSize);
}

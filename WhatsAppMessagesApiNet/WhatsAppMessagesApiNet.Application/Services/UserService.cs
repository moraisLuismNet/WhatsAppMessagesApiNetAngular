using AutoMapper;
using WhatsAppMessagesApiNet.Application.DTOs.User;
using WhatsAppMessagesApiNet.Application.DTOs.Common;
using WhatsAppMessagesApiNet.Application.Interfaces;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Interfaces;

namespace WhatsAppMessagesApiNet.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.Email = dto.Email;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(string email, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email) ?? throw new KeyNotFoundException($"User with email {email} not found");
        _mapper.Map(dto, user);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email) ?? throw new KeyNotFoundException($"User with email {email} not found");
        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PagedResultDto<UserDto>> GetPagedAsync(int page, int pageSize)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var list = users.ToList();
        var items = _mapper.Map<IEnumerable<UserDto>>(list.Skip((page - 1) * pageSize).Take(pageSize));
        return new PagedResultDto<UserDto>
        {
            Items = items,
            TotalCount = list.Count,
            Page = page,
            PageSize = pageSize
        };
    }
}

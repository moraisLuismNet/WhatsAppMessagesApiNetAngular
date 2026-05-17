using AutoMapper;
using WhatsAppMessagesApiNet.Application.DTOs.Message;
using WhatsAppMessagesApiNet.Application.DTOs.Common;
using WhatsAppMessagesApiNet.Application.Interfaces;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;

namespace WhatsAppMessagesApiNet.Application.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEnumerable<IWhatsAppProvider> _whatsAppProviders;

    public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IEnumerable<IWhatsAppProvider> whatsAppProviders)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _whatsAppProviders = whatsAppProviders;
    }

    public async Task<MessageDto?> GetByIdAsync(int id)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(id);
        return message == null ? null : _mapper.Map<MessageDto>(message);
    }

    public async Task<IEnumerable<MessageDto>> GetAllAsync()
    {
        var messages = await _unitOfWork.Messages.GetAllAsync();
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<MessageDto> CreateAsync(CreateMessageDto dto, string userId)
    {
        // Resolve email to phone number if 'to' is an email
        var to = dto.To;
        if (to.Contains('@'))
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(to);
            if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
                to = user.PhoneNumber;
        }

        var message = new Message
        {
            UserId = userId,
            To = to,
            Body = dto.Body,
            Status = MessageStatus.Pending
        };
        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.CompleteAsync();

        // Send via first available provider
        var provider = _whatsAppProviders.FirstOrDefault();
        if (provider != null)
        {
            var result = await provider.SendTextAsync(to, dto.Body);
            if (result.Success)
            {
                message.Status = MessageStatus.Sent;
                message.SentAt = DateTime.UtcNow;
            }
            else
            {
                message.Status = MessageStatus.Failed;
                message.ErrorMessage = result.ErrorMessage;
            }
            await _unitOfWork.CompleteAsync();
        }

        return _mapper.Map<MessageDto>(message);
    }

    public async Task<MessageDto> UpdateStatusAsync(int id, UpdateMessageStatusDto dto)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Message with id {id} not found");
        message.Status = Enum.Parse<MessageStatus>(dto.Status);
        message.ErrorMessage = dto.ErrorMessage;
        if (message.Status == MessageStatus.Sent)
            message.SentAt = DateTime.UtcNow;
        await _unitOfWork.Messages.UpdateAsync(message);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<MessageDto>(message);
    }

    public async Task DeleteAsync(int id)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Message with id {id} not found");
        await _unitOfWork.Messages.DeleteAsync(message);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<MessageDto>> GetByUserIdAsync(string userId)
    {
        var messages = await _unitOfWork.Messages.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<IEnumerable<MessageDto>> GetByRecipientAsync(string userEmail)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
        var toValues = new List<string> { userEmail };
        if (user?.PhoneNumber != null)
            toValues.Add(user.PhoneNumber);

        var messages = await _unitOfWork.Messages.FindAsync(m => toValues.Contains(m.To));
        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<PagedResultDto<MessageDto>> GetPagedAsync(int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetAllAsync();
        var list = messages.ToList();
        var items = _mapper.Map<IEnumerable<MessageDto>>(list.Skip((page - 1) * pageSize).Take(pageSize));
        return new PagedResultDto<MessageDto>
        {
            Items = items,
            TotalCount = list.Count,
            Page = page,
            PageSize = pageSize
        };
    }
}

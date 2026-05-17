using AutoMapper;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Application.DTOs.User;
using WhatsAppMessagesApiNet.Application.DTOs.Message;
using WhatsAppMessagesApiNet.Application.DTOs.Auth;

namespace WhatsAppMessagesApiNet.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Role, o => o.MapFrom(s => Enum.Parse<UserRole>(s.Role)));
        CreateMap<UpdateUserDto, User>()
            .ForMember(d => d.Role, o => o.MapFrom(s => Enum.Parse<UserRole>(s.Role)));
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.Name : string.Empty));
        CreateMap<RegisterDto, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Role, o => o.MapFrom(s => UserRole.Operator));
    }
}

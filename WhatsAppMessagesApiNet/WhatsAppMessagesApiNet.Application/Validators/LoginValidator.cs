using FluentValidation;
using WhatsAppMessagesApiNet.Application.DTOs.Auth;

namespace WhatsAppMessagesApiNet.Application.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

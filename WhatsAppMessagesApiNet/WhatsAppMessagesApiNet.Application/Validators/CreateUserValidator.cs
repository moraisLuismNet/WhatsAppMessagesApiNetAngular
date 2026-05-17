using FluentValidation;
using WhatsAppMessagesApiNet.Application.DTOs.User;

namespace WhatsAppMessagesApiNet.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.Role).NotEmpty().Must(r => new[] { "Admin", "Operator", "Viewer" }.Contains(r));
    }
}

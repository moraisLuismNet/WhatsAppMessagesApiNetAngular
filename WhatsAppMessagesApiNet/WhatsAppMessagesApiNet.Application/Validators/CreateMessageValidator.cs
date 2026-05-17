using FluentValidation;
using WhatsAppMessagesApiNet.Application.DTOs.Message;

namespace WhatsAppMessagesApiNet.Application.Validators;

public class CreateMessageValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.To).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4096);
    }
}

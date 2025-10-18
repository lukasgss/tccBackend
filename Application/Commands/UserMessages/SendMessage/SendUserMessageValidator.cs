using FluentValidation;

namespace Application.Commands.UserMessages.SendMessage;

public class SendUserMessageValidator : AbstractValidator<SendUserMessageCommand>
{
    public SendUserMessageValidator()
    {
        RuleFor(message => message.Content)
            .NotEmpty()
            .WithMessage("Campo de conteúdo é obrigatório.")
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");

        RuleFor(message => message.ReceiverId)
            .NotEmpty()
            .WithMessage("Campo de recebedor é obrigatório.");

        RuleFor(message => message.SenderId)
            .NotEmpty()
            .WithMessage("Campo de remetente é obrigatório.");
    }
}
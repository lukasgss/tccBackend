using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Application.Commands.UserMessages.EditMessage;

[ExcludeFromCodeCoverage]
public sealed class EditMessageValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageValidator()
    {
        RuleFor(message => message.Id)
            .NotNull()
            .WithMessage("Campo de id é obrigatório.");

        RuleFor(message => message.UserId)
            .NotEmpty()
            .WithMessage("Campo de id do usuário é obrigatório.");

        RuleFor(message => message.Content)
            .NotEmpty()
            .WithMessage("Campo de conteúdo é obrigatório.")
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");
    }
}
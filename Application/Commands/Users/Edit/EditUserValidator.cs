using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Application.Commands.Users.Edit;

[ExcludeFromCodeCoverage]
public sealed class EditUserValidator : AbstractValidator<EditUserCommand>
{
    public EditUserValidator()
    {
        RuleFor(user => user.FullName)
            .NotEmpty()
            .WithMessage("Campo de nome completo é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");

        RuleFor(user => user.PhoneNumber)
            .NotEmpty()
            .WithMessage("Campo de telefone é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");
    }
}
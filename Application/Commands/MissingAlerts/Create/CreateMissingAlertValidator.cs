using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Application.Commands.MissingAlerts.Create;

[ExcludeFromCodeCoverage]
public sealed class CreateMissingAlertValidator : AbstractValidator<CreateMissingAlertCommand>
{
    public CreateMissingAlertValidator()
    {
        RuleFor(alert => alert.City)
            .NotNull()
            .WithMessage("Campo de cidade é obrigatório.")
            .Must(city => city > 0)
            .WithMessage("Campo de cidade inválido");

        RuleFor(alert => alert.State)
            .NotNull()
            .WithMessage("Campo de cidade é obrigatório.")
            .Must(city => city > 0)
            .WithMessage("Campo de cidade inválido.");

        RuleFor(alert => alert.Description)
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");

        RuleFor(alert => alert.UserId)
            .NotEmpty()
            .WithMessage("Campo de dono do alerta é obrigatório.");
    }
}
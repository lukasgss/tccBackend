using System.Diagnostics.CodeAnalysis;
using Application.Common.Validations.PetValidations;
using FluentValidation;

namespace Application.Commands.AdoptionAlerts.CreateAdoptionAlert;

[ExcludeFromCodeCoverage]
public sealed class CreateAdoptionAlertValidator : AbstractValidator<CreateAdoptionAlertCommand>
{
    public CreateAdoptionAlertValidator()
    {
        RuleFor(alert => alert.AdoptionRestrictions)
            .Must(restrictions => restrictions.TrueForAll(restriction => restriction.Trim().Length > 0))
            .WithMessage("Restrição não pode ser vazia.");

        RuleFor(alert => alert.Neighborhood)
            .NotEmpty()
            .WithMessage("Campo de bairro é obrigatório.");

        RuleFor(alert => alert.State)
            .NotEmpty()
            .WithMessage("Campo de estado é obrigatório.");

        RuleFor(alert => alert.City)
            .NotEmpty()
            .WithMessage("Campo de cidade é obrigatório.");

        RuleFor(alert => alert.Description)
            .MaximumLength(1000)
            .WithMessage("Máximo de 1000 caracteres permitidos.");

        RuleFor(alert => alert.Pet).SetValidator(new CreatePetValidator());
    }
}
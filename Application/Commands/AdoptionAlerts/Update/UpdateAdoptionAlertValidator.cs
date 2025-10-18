using Application.Common.Validations.PetValidations;
using FluentValidation;

namespace Application.Commands.AdoptionAlerts.Update;

public class UpdateAdoptionAlertValidator : AbstractValidator<UpdateAdoptionAlertCommand>
{
    public UpdateAdoptionAlertValidator()
    {
        RuleFor(alert => alert.Id)
            .NotEmpty()
            .WithMessage("Campo de id é obrigatório.");

        RuleFor(alert => alert.Neighborhood)
            .NotEmpty()
            .WithMessage("Campo de bairro é obrigatório.");

        RuleFor(alert => alert.AdoptionRestrictions)
            .Must(restrictions => restrictions.TrueForAll(restriction => restriction.Length > 0))
            .WithMessage("Restrição não pode ser vazia.");

        RuleFor(alert => alert.Description)
            .MaximumLength(1000)
            .WithMessage("Máximo de 1000 caracteres permitidos.");

        RuleFor(alert => alert.Pet)
            .NotEmpty()
            .WithMessage("Campo do pet é obrigatório.")
            .SetValidator(new EditPetValidator());
    }
}
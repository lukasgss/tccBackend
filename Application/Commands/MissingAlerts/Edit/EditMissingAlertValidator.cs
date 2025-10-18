using FluentValidation;

namespace Application.Commands.MissingAlerts.Edit;

public class EditMissingAlertValidator : AbstractValidator<EditMissingAlertCommand>
{
    public EditMissingAlertValidator()
    {
        RuleFor(alert => alert.Id)
            .NotEmpty()
            .WithMessage("Campo de id é obrigatório.");

        RuleFor(alert => alert.LastSeenLocationLatitude)
            .NotNull()
            .WithMessage("Campo de latitude é obrigatório.")
            .Must(latitude => latitude >= -90 && latitude <= 90)
            .WithMessage("Campo de latitude deve ser entre -90 e 90.");

        RuleFor(alert => alert.LastSeenLocationLongitude)
            .NotNull()
            .WithMessage("Campo de longitude é obrigatório.")
            .Must(longitude => longitude >= -180 && longitude <= 180)
            .WithMessage("Campo de latitude deve ser entre -180 e 180.");

        RuleFor(alert => alert.Description)
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");

        RuleFor(alert => alert.PetId)
            .NotEmpty()
            .WithMessage("Campo do pet é obrigatório.");
    }
}
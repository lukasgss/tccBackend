using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Application.Commands.AdoptionAlerts.ReportAdoptionAlert;

[ExcludeFromCodeCoverage]
public sealed class ReportAdoptionAlertValidator : AbstractValidator<ReportAdoptionAlertCommand>
{
    public ReportAdoptionAlertValidator()
    {
        RuleFor(p => p.Reason)
            .NotEmpty()
            .WithMessage("Campo de motivo da denúncia é obrigatório.")
            .MaximumLength(50)
            .WithMessage("Campo de motivo da denúncia pode ter no máximo 50 caracteres.");

        RuleFor(p => p.AlertId)
            .NotEmpty()
            .WithMessage("É obrigatório especificar a adoção.");
    }
}
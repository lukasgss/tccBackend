using FluentValidation;

namespace Application.Commands.Users.RedefinePassword;

public class RedefinePasswordValidator : AbstractValidator<RedefinePasswordCommand>
{
    public RedefinePasswordValidator()
    {
        RuleFor(user => user.NewPassword)
            .NotEmpty()
            .WithMessage("Campo de senha é obrigatório.")
            .MinimumLength(6)
            .WithMessage("A senha deve possuir no mínimo 6 caracteres.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");

        RuleFor(user => user.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Campo de confirmar senha é obrigatório.")
            .Equal(user => user.NewPassword)
            .WithMessage("Campo de senha e confirmar senha devem ser iguais.");

        RuleFor(x => x.ResetCode)
            .NotEmpty()
            .WithMessage("Token de redefinição é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Endereço de e-mail inválido.");
    }
}
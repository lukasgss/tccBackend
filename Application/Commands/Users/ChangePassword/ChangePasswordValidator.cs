using FluentValidation;

namespace Application.Commands.Users.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
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

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Senha atual é obrigatória.");
    }
}
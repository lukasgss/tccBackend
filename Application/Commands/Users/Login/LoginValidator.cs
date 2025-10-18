using FluentValidation;

namespace Application.Commands.Users.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Campo de e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Endereço de e-mail inválido.");

        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Campo de senha é obrigatório.");
    }
}
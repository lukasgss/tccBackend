using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Users;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Users.RedefinePassword;

public record RedefinePasswordCommand(
    string Email,
    string ResetCode,
    string NewPassword,
    string ConfirmNewPassword)
    : IRequest<Unit>;

public class RedefinePasswordCommandHandler : IRequestHandler<RedefinePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public RedefinePasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = Guard.Against.Null(userRepository);
    }

    public async Task<Unit> Handle(RedefinePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new UnauthorizedException("Não foi possível redefinir a senha do usuário");
        }

        IdentityResult resetPasswordResult = await _userRepository.ResetPasswordAsync(user,
            request.ResetCode,
            request.NewPassword);

        if (!resetPasswordResult.Succeeded)
        {
            throw new UnauthorizedException("Não foi possível redefinir a senha do usuário");
        }

        return Unit.Value;
    }
}
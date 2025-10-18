using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Users;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Users.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword,
    Guid UserId) : IRequest<Unit>;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public ChangePasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = Guard.Against.Null(userRepository);
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            throw new BadRequestException("Não foi possível redefinir a senha do usuário.");
        }

        IdentityResult credentialsCheck = await _userRepository.ChangePasswordAsync(user,
            request.CurrentPassword,
            request.NewPassword);
        if (!credentialsCheck.Succeeded)
        {
            throw new BadRequestException("Senha atual não coincide com a senha inserida.");
        }

        return Unit.Value;
    }
}
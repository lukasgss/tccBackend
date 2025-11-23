using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Users;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Users.ConfirmEmail;

[ExcludeFromCodeCoverage]
public sealed record ConfirmEmailCommand(string HashedUserId, string Token) : IRequest<Unit>;

public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly IIdConverterService _idConverterService;
    private readonly IUserRepository _userRepository;

    public ConfirmEmailCommandHandler(IIdConverterService idConverterService, IUserRepository userRepository)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _idConverterService = Guard.Against.Null(idConverterService);
    }

    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        Guid decodedUserId = _idConverterService.DecodeShortIdToGuid(request.HashedUserId);

        User? user = await _userRepository.GetUserByIdAsync(decodedUserId);
        if (user is null)
        {
            throw new BadRequestException("Não foi possível ativar o email com os dados informados.");
        }

        IdentityResult result = await _userRepository.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            throw new BadRequestException("Não foi possível ativar o email com os dados informados.");
        }

        return Unit.Value;
    }
}
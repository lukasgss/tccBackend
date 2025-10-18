using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.Users.RefreshToken;

public record RefreshTokenCommand(Guid UserId) : IRequest<TokensResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokensResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ILogger<RefreshTokenCommandHandler> logger,
        ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = Guard.Against.Null(tokenGenerator);
        _logger = Guard.Against.Null(logger);
        _userRepository = Guard.Against.Null(userRepository);
    }

    public async Task<TokensResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            _logger.LogInformation("Usuário {UserId} não existe", request.UserId);
            throw new NotFoundException("Não foi possível encontrar um usuário com esse id.");
        }

        if (await _userRepository.IsLockedOutAsync(user))
        {
            throw new LockedException("A conta está bloqueada.");
        }

        return _tokenGenerator.GenerateTokens(request.UserId, user.FullName);
    }
}
using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Users.Login;

[ExcludeFromCodeCoverage]
public sealed record LoginCommand(string Email, string Password) : IRequest<UserResponse>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginCommandHandler(IUserRepository userRepository, ITokenGenerator tokenGenerator)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _tokenGenerator = Guard.Against.Null(tokenGenerator);
    }

    public async Task<UserResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? userToLogin = await _userRepository.GetUserByEmailAsync(request.Email);
        if (userToLogin is null)
        {
            // The userToLogin object is assigned to avoid time based attacks where
            // it's possible to enumerate valid emails based on response times from
            // the server
            userToLogin = new User()
            {
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        SignInResult signInResult = await _userRepository.CheckCredentials(userToLogin, request.Password);

        if (!signInResult.Succeeded || userToLogin is null)
        {
            if (signInResult.IsLockedOut)
            {
                throw new LockedException("Essa conta está bloqueada, aguarde e tente novamente.");
            }

            throw new UnauthorizedException("Credenciais inválidas.");
        }

        TokensResponse tokens = _tokenGenerator.GenerateTokens(userToLogin.Id, userToLogin.FullName);

        return userToLogin.ToUserResponse(tokens);
    }
}
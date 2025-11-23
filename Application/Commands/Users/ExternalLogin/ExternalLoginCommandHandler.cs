using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Queries.Users.Images.ValidateImage;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Users.ExternalLogin;

[ExcludeFromCodeCoverage]
public sealed record ExternalLoginCommand(string Provider, string IdToken) : IRequest<UserResponse>;

[ExcludeFromCodeCoverage]
public sealed class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, UserResponse>
{
    private const string GoogleProvider = "GOOGLE";
    private const string FacebookProvider = "FACEBOOK";

    private readonly IExternalAuthHandler _externalAuthHandler;
    private readonly IUserRepository _userRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ISender _mediator;

    public ExternalLoginCommandHandler(
        IExternalAuthHandler externalAuthHandler,
        IUserRepository userRepository,
        IValueProvider valueProvider,
        ITokenGenerator tokenGenerator,
        ISender mediator)
    {
        _externalAuthHandler = Guard.Against.Null(externalAuthHandler);
        _userRepository = Guard.Against.Null(userRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _tokenGenerator = Guard.Against.Null(tokenGenerator);
        _mediator = Guard.Against.Null(mediator);
    }

    public async Task<UserResponse> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        return request.Provider.ToUpperInvariant() switch
        {
            GoogleProvider => await GoogleLoginAsync(request),
            FacebookProvider => await FacebookLoginAsync(request),
            _ => throw new BadRequestException("Provedor de autenticação externa não suportada.")
        };
    }

    private async Task<UserResponse> GoogleLoginAsync(ExternalLoginCommand externalAuth)
    {
        ExternalAuthPayload? payload = await _externalAuthHandler.ValidateGoogleToken(externalAuth);
        if (payload is null)
        {
            throw new BadRequestException("Não foi possível realizar o login com o Google.");
        }

        UserLoginInfo info = new(GoogleProvider, payload.UserId, GoogleProvider);

        User user = await RegisterUserFromExternalAuthProviderAsync(payload, info);

        TokensResponse tokens = _tokenGenerator.GenerateTokens(user.Id, user.FullName);

        return user.ToUserResponse(tokens);
    }

    private async Task<UserResponse> FacebookLoginAsync(ExternalLoginCommand externalAuth)
    {
        ExternalAuthPayload? payload = await _externalAuthHandler.ValidateFacebookToken(externalAuth);
        if (payload is null)
        {
            throw new BadRequestException("Não foi possível realizar o login com o Facebook.");
        }

        UserLoginInfo info = new(FacebookProvider, payload.UserId, FacebookProvider);

        User user = await RegisterUserFromExternalAuthProviderAsync(payload, info);

        TokensResponse tokens = _tokenGenerator.GenerateTokens(user.Id, user.FullName);

        return user.ToUserResponse(tokens);
    }

    private async Task<User> RegisterUserFromExternalAuthProviderAsync(ExternalAuthPayload payload, UserLoginInfo info)
    {
        User? user = await _userRepository.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user is not null)
        {
            return user;
        }

        user = await _userRepository.FindByEmailAsync(payload.Email);
        if (user is null)
        {
            ValidateUserImageQuery validateImageQuery = new(payload.Image);
            string userImageUrl = await _mediator.Send(validateImageQuery);
            user = new User
            {
                Id = _valueProvider.NewGuid(),
                Email = payload.Email,
                UserName = payload.Email,
                FullName = payload.FullName,
                Image = userImageUrl,
                EmailConfirmed = true
            };
            await _userRepository.RegisterUserAsync(user);
        }

        await _userRepository.AddLoginAsync(user, info);

        return user;
    }
}
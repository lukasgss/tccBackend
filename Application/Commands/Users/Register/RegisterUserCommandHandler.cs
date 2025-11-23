using System.Diagnostics.CodeAnalysis;
using Application.Commands.Users.Images.Upload;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Users.Register;

[ExcludeFromCodeCoverage]
public sealed record RegisterUserCommand(
    string FullName,
    string PhoneNumber,
    string Email,
    bool OnlyWhatsAppMessages,
    string Password,
    string ConfirmPassword
) : IRequest<UserResponse>;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponse>
{
    private readonly IValueProvider _valueProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserDao _userDao;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ISender _mediator;

    public RegisterUserCommandHandler(
        IValueProvider valueProvider,
        IUserRepository userRepository,
        IUserDao userDao,
        ITokenGenerator tokenGenerator,
        ISender mediator)
    {
        _valueProvider = Guard.Against.Null(valueProvider);
        _mediator = Guard.Against.Null(mediator);
        _tokenGenerator = Guard.Against.Null(tokenGenerator);
        _userDao = Guard.Against.Null(userDao);
        _userRepository = Guard.Against.Null(userRepository);
    }

    public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Guid userId = _valueProvider.NewGuid();

        UploadUserImageCommand uploadImageCommand = new(null, null);
        string userImageUrl = await _mediator.Send(uploadImageCommand, cancellationToken);

        User userToCreate = new()
        {
            Id = userId,
            FullName = request.FullName,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            ReceivesOnlyWhatsAppMessages = request.OnlyWhatsAppMessages,
            Image = userImageUrl,
            Email = request.Email,
            EmailConfirmed = true
        };

        var userAlreadyExists = await _userDao.UserWithEmailExists(request.Email);
        if (userAlreadyExists)
        {
            throw new ConflictException("Usuário com o e-mail especificado já existe.");
        }

        IdentityResult registrationResult =
            await _userRepository.RegisterUserAsync(userToCreate, request.Password);

        IdentityResult lockoutResult = await _userRepository.SetLockoutEnabledAsync(userToCreate, false);
        if (!registrationResult.Succeeded || !lockoutResult.Succeeded)
        {
            throw new InternalServerErrorException();
        }

        TokensResponse tokens = _tokenGenerator.GenerateTokens(userToCreate.Id, userToCreate.FullName);

        return userToCreate.ToUserResponse(tokens);
    }
}
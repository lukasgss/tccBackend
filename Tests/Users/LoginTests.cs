using Application.Commands.Users.Login;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Tests.Users;

public sealed class LoginCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly ITokenGenerator _tokenGenerator;
	private readonly LoginCommandHandler _handler;

	public LoginCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();
		_tokenGenerator = Substitute.For<ITokenGenerator>();

		_handler = new LoginCommandHandler(_userRepository, _tokenGenerator);
	}

	[Fact]
	public async Task Handle_WhenUserNotFoundAndCredentialsFail_ShouldThrowUnauthorizedException()
	{
		// Arrange
		var command = new LoginCommand("nonexistent@email.com", "password");

		_userRepository.GetUserByEmailAsync(command.Email).Returns((User?)null);
		_userRepository.CheckCredentials(Arg.Any<User>(), command.Password)
			.Returns(SignInResult.Failed);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Credenciais inválidas.");
	}

	[Fact]
	public async Task Handle_WhenCredentialsInvalid_ShouldThrowUnauthorizedException()
	{
		// Arrange
		var command = new LoginCommand("user@email.com", "wrongpassword");
		var user = CreateUser();

		_userRepository.GetUserByEmailAsync(command.Email).Returns(user);
		_userRepository.CheckCredentials(user, command.Password)
			.Returns(SignInResult.Failed);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Credenciais inválidas.");
	}

	[Fact]
	public async Task Handle_WhenAccountIsLockedOut_ShouldThrowLockedException()
	{
		// Arrange
		var command = new LoginCommand("user@email.com", "password");
		var user = CreateUser();

		_userRepository.GetUserByEmailAsync(command.Email).Returns(user);
		_userRepository.CheckCredentials(user, command.Password)
			.Returns(SignInResult.LockedOut);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<LockedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Essa conta está bloqueada, aguarde e tente novamente.");
	}

	[Fact]
	public async Task Handle_WhenCredentialsValid_ShouldReturnUserResponse()
	{
		// Arrange
		var command = new LoginCommand("user@email.com", "correctpassword");
		var user = CreateUser();
		var tokens = new TokensResponse
		{
			AccessToken = "access-token",
			RefreshToken = "refresh-token"
		};

		_userRepository.GetUserByEmailAsync(command.Email).Returns(user);
		_userRepository.CheckCredentials(user, command.Password)
			.Returns(SignInResult.Success);
		_tokenGenerator.GenerateTokens(user.Id, user.FullName).Returns(tokens);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		_tokenGenerator.Received(1).GenerateTokens(user.Id, user.FullName);
	}

	private static User CreateUser()
	{
		return new User
		{
			Id = Guid.NewGuid(),
			Email = "user@email.com",
			ReceivesOnlyWhatsAppMessages = true,
			PhoneNumber = "123456789",
			FullName = "Test User",
			Image = "image.jpg",
			DefaultAdoptionFormUrl = "url",
			SecurityStamp = Guid.NewGuid().ToString()
		};
	}
}
using Application.Commands.Users.RefreshToken;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Tests.Users;

public sealed class RefreshTokenCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly ITokenGenerator _tokenGenerator;
	private readonly RefreshTokenCommandHandler _handler;

	public RefreshTokenCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();
		_tokenGenerator = Substitute.For<ITokenGenerator>();
		var logger = Substitute.For<ILogger<RefreshTokenCommandHandler>>();

		_handler = new RefreshTokenCommandHandler(
			_userRepository,
			logger,
			_tokenGenerator);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new RefreshTokenCommand(Guid.NewGuid());
		_userRepository.GetUserByIdAsync(command.UserId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível encontrar um usuário com esse id.");
	}

	[Fact]
	public async Task Handle_WhenUserIsLockedOut_ShouldThrowLockedException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = new RefreshTokenCommand(userId);
		var user = CreateUser(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_userRepository.IsLockedOutAsync(user).Returns(true);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<LockedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("A conta está bloqueada.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldReturnTokens()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = new RefreshTokenCommand(userId);
		var user = CreateUser(userId);
		var tokens = new TokensResponse
		{
			AccessToken = "access-token",
			RefreshToken = "refresh-token"
		};

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_userRepository.IsLockedOutAsync(user).Returns(false);
		_tokenGenerator.GenerateTokens(userId, user.FullName).Returns(tokens);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(tokens);
		_tokenGenerator.Received(1).GenerateTokens(userId, user.FullName);
	}

	private static User CreateUser(Guid userId)
	{
		return new User
		{
			Id = userId,
			Email = "user@email.com",
			ReceivesOnlyWhatsAppMessages = true,
			PhoneNumber = "123456789",
			FullName = "Test User",
			Image = "image.jpg",
			DefaultAdoptionFormUrl = "url"
		};
	}
}
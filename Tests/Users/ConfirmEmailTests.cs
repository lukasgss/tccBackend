using Application.Commands.Users.ConfirmEmail;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Tests.Users;

public sealed class ConfirmEmailCommandHandlerTests
{
	private readonly IIdConverterService _idConverterService;
	private readonly IUserRepository _userRepository;
	private readonly ConfirmEmailCommandHandler _handler;

	public ConfirmEmailCommandHandlerTests()
	{
		_idConverterService = Substitute.For<IIdConverterService>();
		_userRepository = Substitute.For<IUserRepository>();

		_handler = new ConfirmEmailCommandHandler(_idConverterService, _userRepository);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowBadRequestException()
	{
		// Arrange
		var hashedUserId = "abc123";
		var token = "token123";
		var decodedUserId = Guid.NewGuid();
		var command = new ConfirmEmailCommand(hashedUserId, token);

		_idConverterService.DecodeShortIdToGuid(hashedUserId).Returns(decodedUserId);
		_userRepository.GetUserByIdAsync(decodedUserId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível ativar o email com os dados informados.");
	}

	[Fact]
	public async Task Handle_WhenConfirmEmailFails_ShouldThrowBadRequestException()
	{
		// Arrange
		var hashedUserId = "abc123";
		var token = "invalidToken";
		var decodedUserId = Guid.NewGuid();
		var command = new ConfirmEmailCommand(hashedUserId, token);
		var user = CreateUser(decodedUserId);

		_idConverterService.DecodeShortIdToGuid(hashedUserId).Returns(decodedUserId);
		_userRepository.GetUserByIdAsync(decodedUserId).Returns(user);
		_userRepository.ConfirmEmailAsync(user, token)
			.Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível ativar o email com os dados informados.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldConfirmEmail()
	{
		// Arrange
		var hashedUserId = "abc123";
		var token = "validToken";
		var decodedUserId = Guid.NewGuid();
		var command = new ConfirmEmailCommand(hashedUserId, token);
		var user = CreateUser(decodedUserId);

		_idConverterService.DecodeShortIdToGuid(hashedUserId).Returns(decodedUserId);
		_userRepository.GetUserByIdAsync(decodedUserId).Returns(user);
		_userRepository.ConfirmEmailAsync(user, token).Returns(IdentityResult.Success);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _userRepository.Received(1).ConfirmEmailAsync(user, token);
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
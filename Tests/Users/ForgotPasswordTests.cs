using Application.Commands.Users.ForgotPassword;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Messaging;
using Domain.Entities;

namespace Tests.Users;

public sealed class ForgotPasswordCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly IMessagingService _messagingService;
	private readonly ForgotPasswordCommandHandler _handler;

	public ForgotPasswordCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();
		_messagingService = Substitute.For<IMessagingService>();

		_handler = new ForgotPasswordCommandHandler(_userRepository, _messagingService);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldReturnWithoutSendingEmail()
	{
		// Arrange
		var command = new ForgotPasswordCommand("nonexistent@email.com");
		_userRepository.FindByEmailAsync(command.Email).Returns((User?)null);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _userRepository.DidNotReceive().GeneratePasswordResetTokenAsync(Arg.Any<User>());
		await _messagingService.DidNotReceive()
			.SendForgotPasswordMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
	}

	[Fact]
	public async Task Handle_WhenUserFound_ShouldGenerateTokenAndSendEmail()
	{
		// Arrange
		var email = "user@email.com";
		var command = new ForgotPasswordCommand(email);
		var user = CreateUser(email);
		var token = "reset-token-123";

		_userRepository.FindByEmailAsync(email).Returns(user);
		_userRepository.GeneratePasswordResetTokenAsync(user).Returns(token);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _userRepository.Received(1).GeneratePasswordResetTokenAsync(user);
		await _messagingService.Received(1)
			.SendForgotPasswordMessageAsync(user.Email!, user.FullName, token);
	}

	private static User CreateUser(string email)
	{
		return new User
		{
			Id = Guid.NewGuid(),
			Email = email,
			ReceivesOnlyWhatsAppMessages = true,
			PhoneNumber = "123456789",
			FullName = "Test User",
			Image = "image.jpg",
			DefaultAdoptionFormUrl = "url"
		};
	}
}
using Application.Commands.Users.RedefinePassword;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Tests.Users;

public sealed class RedefinePasswordCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly RedefinePasswordCommandHandler _handler;

	public RedefinePasswordCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();

		_handler = new RedefinePasswordCommandHandler(_userRepository);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowUnauthorizedException()
	{
		// Arrange
		var command = new RedefinePasswordCommand(
			"nonexistent@email.com",
			"reset-code",
			"newPassword",
			"newPassword");

		_userRepository.FindByEmailAsync(command.Email).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível redefinir a senha do usuário");
	}

	[Fact]
	public async Task Handle_WhenResetPasswordFails_ShouldThrowUnauthorizedException()
	{
		// Arrange
		var command = new RedefinePasswordCommand(
			"user@email.com",
			"invalid-reset-code",
			"newPassword",
			"newPassword");
		var user = CreateUser();

		_userRepository.FindByEmailAsync(command.Email).Returns(user);
		_userRepository.ResetPasswordAsync(user, command.ResetCode, command.NewPassword)
			.Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

		// Act & Assert
		var exception =
			await Should.ThrowAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível redefinir a senha do usuário");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldResetPassword()
	{
		// Arrange
		var command = new RedefinePasswordCommand(
			"user@email.com",
			"valid-reset-code",
			"newPassword",
			"newPassword");
		var user = CreateUser();

		_userRepository.FindByEmailAsync(command.Email).Returns(user);
		_userRepository.ResetPasswordAsync(user, command.ResetCode, command.NewPassword)
			.Returns(IdentityResult.Success);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		await _userRepository.Received(1).ResetPasswordAsync(user, command.ResetCode, command.NewPassword);
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
			DefaultAdoptionFormUrl = "url"
		};
	}
}
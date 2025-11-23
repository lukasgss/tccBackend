using Application.Commands.Users.ChangePassword;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Tests.Users;

public sealed class ChangePasswordCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly ChangePasswordCommandHandler _handler;

	public ChangePasswordCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();

		_handler = new ChangePasswordCommandHandler(_userRepository);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowBadRequestException()
	{
		// Arrange
		var command = new ChangePasswordCommand("currentPass", "newPass", "newPass", Guid.NewGuid());
		_userRepository.GetUserByIdAsync(command.UserId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível redefinir a senha do usuário.");
	}

	[Fact]
	public async Task Handle_WhenCurrentPasswordIsWrong_ShouldThrowBadRequestException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = new ChangePasswordCommand("wrongPassword", "newPass", "newPass", userId);
		var user = CreateUser(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_userRepository.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword)
			.Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid password" }));

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Senha atual não coincide com a senha inserida.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldChangePassword()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = new ChangePasswordCommand("currentPass", "newPass", "newPass", userId);
		var user = CreateUser(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_userRepository.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword)
			.Returns(IdentityResult.Success);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		await _userRepository.Received(1).ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
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
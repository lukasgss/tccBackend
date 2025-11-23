using Application.Commands.Users.Images.Upload;
using Application.Commands.Users.Register;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Tests.Users;

public sealed class RegisterUserCommandHandlerTests
{
	private readonly IValueProvider _valueProvider;
	private readonly IUserRepository _userRepository;
	private readonly IUserDao _userDao;
	private readonly ITokenGenerator _tokenGenerator;
	private readonly ISender _mediator;
	private readonly RegisterUserCommandHandler _handler;

	public RegisterUserCommandHandlerTests()
	{
		_valueProvider = Substitute.For<IValueProvider>();
		_userRepository = Substitute.For<IUserRepository>();
		_userDao = Substitute.For<IUserDao>();
		_tokenGenerator = Substitute.For<ITokenGenerator>();
		_mediator = Substitute.For<ISender>();

		_handler = new RegisterUserCommandHandler(
			_valueProvider,
			_userRepository,
			_userDao,
			_tokenGenerator,
			_mediator);
	}

	[Fact]
	public async Task Handle_WhenUserAlreadyExists_ShouldThrowConflictException()
	{
		// Arrange
		var command = CreateCommand();
		var userId = Guid.NewGuid();
		var imageUrl = "http://default-image.jpg";

		_valueProvider.NewGuid().Returns(userId);
		_mediator.Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>())
			.Returns(imageUrl);
		_userDao.UserWithEmailExists(command.Email).Returns(true);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Usuário com o e-mail especificado já existe.");
	}

	[Fact]
	public async Task Handle_WhenRegistrationFails_ShouldThrowInternalServerErrorException()
	{
		// Arrange
		var command = CreateCommand();
		var userId = Guid.NewGuid();
		var imageUrl = "http://default-image.jpg";

		_valueProvider.NewGuid().Returns(userId);
		_mediator.Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>())
			.Returns(imageUrl);
		_userDao.UserWithEmailExists(command.Email).Returns(false);
		_userRepository.RegisterUserAsync(Arg.Any<User>(), command.Password)
			.Returns(IdentityResult.Failed(new IdentityError { Description = "Registration failed" }));

		// Act & Assert
		await Should.ThrowAsync<InternalServerErrorException>(() => _handler.Handle(command, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_WhenLockoutSettingFails_ShouldThrowInternalServerErrorException()
	{
		// Arrange
		var command = CreateCommand();
		var userId = Guid.NewGuid();
		var imageUrl = "http://default-image.jpg";

		_valueProvider.NewGuid().Returns(userId);
		_mediator.Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>())
			.Returns(imageUrl);
		_userDao.UserWithEmailExists(command.Email).Returns(false);
		_userRepository.RegisterUserAsync(Arg.Any<User>(), command.Password)
			.Returns(IdentityResult.Success);
		_userRepository.SetLockoutEnabledAsync(Arg.Any<User>(), false)
			.Returns(IdentityResult.Failed(new IdentityError { Description = "Lockout setting failed" }));

		// Act & Assert
		await Should.ThrowAsync<InternalServerErrorException>(() => _handler.Handle(command, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldRegisterUserAndReturnResponse()
	{
		// Arrange
		var command = CreateCommand();
		var userId = Guid.NewGuid();
		var imageUrl = "http://default-image.jpg";
		var tokens = new TokensResponse
		{
			AccessToken = "access-token",
			RefreshToken = "refresh-token"
		};

		_valueProvider.NewGuid().Returns(userId);
		_mediator.Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>())
			.Returns(imageUrl);
		_userDao.UserWithEmailExists(command.Email).Returns(false);
		_userRepository.RegisterUserAsync(Arg.Any<User>(), command.Password)
			.Returns(IdentityResult.Success);
		_userRepository.SetLockoutEnabledAsync(Arg.Any<User>(), false)
			.Returns(IdentityResult.Success);
		_tokenGenerator.GenerateTokens(userId, command.FullName).Returns(tokens);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		await _userRepository.Received(1).RegisterUserAsync(
			Arg.Is<User>(u =>
				u.Id == userId &&
				u.FullName == command.FullName &&
				u.Email == command.Email &&
				u.PhoneNumber == command.PhoneNumber &&
				u.ReceivesOnlyWhatsAppMessages == command.OnlyWhatsAppMessages &&
				u.Image == imageUrl),
			command.Password);
		_tokenGenerator.Received(1).GenerateTokens(userId, command.FullName);
	}

	private static RegisterUserCommand CreateCommand()
	{
		return new RegisterUserCommand(
			FullName: "Test User",
			PhoneNumber: "123456789",
			Email: "test@email.com",
			OnlyWhatsAppMessages: true,
			Password: "Password123!",
			ConfirmPassword: "Password123!"
		);
	}
}
using Application.Commands.Users.Edit;
using Application.Commands.Users.Images.Upload;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Tests.Users;

public sealed class EditUserCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly ISender _mediator;
	private readonly IAdoptionAlertFileSubmissionService _adoptionAlertFileSubmissionService;
	private readonly EditUserCommandHandler _handler;

	public EditUserCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();
		_mediator = Substitute.For<ISender>();
		_adoptionAlertFileSubmissionService = Substitute.For<IAdoptionAlertFileSubmissionService>();
		var logger = Substitute.For<ILogger<EditUserCommandHandler>>();

		_handler = new EditUserCommandHandler(
			_userRepository,
			_mediator,
			logger,
			_adoptionAlertFileSubmissionService);
	}

	[Fact]
	public async Task Handle_WhenUserIdDoesNotMatchLoggedInUserId_ShouldThrowForbiddenException()
	{
		// Arrange
		var loggedInUserId = Guid.NewGuid();
		var differentUserId = Guid.NewGuid();
		var command = CreateCommand(loggedInUserId, differentUserId);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Você não possui permissão para editar este usuário.");
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = CreateCommand(userId, userId);

		_userRepository.GetUserByIdAsync(userId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Usuário com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenImageProvided_ShouldUploadNewImage()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var newImage = Substitute.For<IFormFile>();
		var command = CreateCommand(userId, userId, image: newImage);
		var user = CreateUser(userId);
		var newImageUrl = "http://new-image.jpg";

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_mediator.Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>())
			.Returns(newImageUrl);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		user.Image.ShouldBe(newImageUrl);
		await _mediator.Received(1).Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>());
		await _userRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenNoImageProvided_ShouldUseExistingImage()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var existingImageUrl = "http://existing-image.jpg";
		var command = CreateCommand(userId, userId, image: null, existingImage: existingImageUrl);
		var user = CreateUser(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		user.Image.ShouldBe(existingImageUrl);
		await _mediator.DidNotReceive().Send(Arg.Any<UploadUserImageCommand>(), Arg.Any<CancellationToken>());
		await _userRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenDefaultAdoptionFormProvided_ShouldUploadForm()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var adoptionForm = Substitute.For<IFormFile>();
		var command = CreateCommand(userId, userId, defaultAdoptionForm: adoptionForm);
		var user = CreateUser(userId);
		var formUrl = "http://adoption-form.pdf";

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_adoptionAlertFileSubmissionService
			.UploadUserDefaultAdoptionFormAsync(adoptionForm, user.DefaultAdoptionFormUrl)
			.Returns(formUrl);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		user.DefaultAdoptionFormUrl.ShouldBe(formUrl);
		await _adoptionAlertFileSubmissionService.Received(1)
			.UploadUserDefaultAdoptionFormAsync(adoptionForm, Arg.Any<string?>());
		await _userRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldUpdateUserAndReturnResponse()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = CreateCommand(userId, userId);
		var user = CreateUser(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		user.FullName.ShouldBe(command.FullName);
		user.PhoneNumber.ShouldBe(command.PhoneNumber);
		user.ReceivesOnlyWhatsAppMessages.ShouldBe(command.OnlyWhatsAppMessages);
		await _userRepository.Received(1).CommitAsync();
	}

	private static EditUserCommand CreateCommand(
		Guid loggedInUserId,
		Guid userId,
		IFormFile? image = null,
		string existingImage = "http://existing.jpg",
		IFormFile? defaultAdoptionForm = null)
	{
		return new EditUserCommand(
			LoggedInUserId: loggedInUserId,
			UserId: userId,
			FullName: "Updated Name",
			PhoneNumber: "987654321",
			OnlyWhatsAppMessages: true,
			Image: image,
			ExistingImage: existingImage,
			DefaultAdoptionForm: defaultAdoptionForm
		);
	}

	private static User CreateUser(Guid userId)
	{
		return new User
		{
			Id = userId,
			Email = "user@email.com",
			ReceivesOnlyWhatsAppMessages = false,
			PhoneNumber = "123456789",
			FullName = "Original Name",
			Image = "http://original-image.jpg",
			DefaultAdoptionFormUrl = "http://original-form.pdf"
		};
	}
}
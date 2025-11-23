using Application.Commands.UserMessages.SendMessage;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Tests.UserMessages;

public sealed class SendUserMessageCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly IUserMessageRepository _userMessageRepository;
	private readonly IValueProvider _valueProvider;
	private readonly SendUserMessageCommandHandler _handler;

	public SendUserMessageCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();
		_userMessageRepository = Substitute.For<IUserMessageRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		var logger = Substitute.For<ILogger<SendUserMessageCommandHandler>>();

		_handler = new SendUserMessageCommandHandler(
			_userRepository,
			_userMessageRepository,
			_valueProvider,
			logger);
	}

	[Fact]
	public async Task Handle_WhenReceiverNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new SendUserMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "Test message");
		_userRepository.GetUserByIdAsync(command.ReceiverId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Usuário destinatário não foi encontrado.");
	}

	[Fact]
	public async Task Handle_WhenSenderNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new SendUserMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "Test message");
		var receiver = CreateUser(command.ReceiverId);

		_userRepository.GetUserByIdAsync(command.ReceiverId).Returns(receiver);
		_userRepository.GetUserByIdAsync(command.SenderId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Usuário remetente não foi encontrado.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldCreateMessageAndReturnResponse()
	{
		// Arrange
		var senderId = Guid.NewGuid();
		var receiverId = Guid.NewGuid();
		var content = "Test message";
		var command = new SendUserMessageCommand(senderId, receiverId, content);
		var now = DateTime.UtcNow;

		var sender = CreateUser(senderId);
		var receiver = CreateUser(receiverId);

		_userRepository.GetUserByIdAsync(receiverId).Returns(receiver);
		_userRepository.GetUserByIdAsync(senderId).Returns(sender);
		_valueProvider.UtcNow().Returns(now);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		_userMessageRepository.Received(1).Add(Arg.Is<UserMessage>(m =>
			m.Content == content &&
			m.Sender == sender &&
			m.Receiver == receiver &&
			m.HasBeenRead == false &&
			m.HasBeenEdited == false &&
			m.TimeStampUtc == now));
		await _userMessageRepository.Received(1).CommitAsync();
	}

	private static User CreateUser(Guid userId)
	{
		return new User
		{
			Id = userId,
			Email = $"user{userId}@email.com",
			ReceivesOnlyWhatsAppMessages = true,
			PhoneNumber = "123456789",
			FullName = "Test User",
			Image = "image.jpg",
			DefaultAdoptionFormUrl = "url"
		};
	}
}
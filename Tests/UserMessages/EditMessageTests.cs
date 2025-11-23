using Application.Commands.UserMessages.EditMessage;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Providers;
using Application.Common.Interfaces.RealTimeCommunication;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Tests.UserMessages;

public sealed class EditMessageCommandHandlerTests
{
	private readonly IUserMessageRepository _userMessageRepository;
	private readonly IValueProvider _valueProvider;
	private readonly IRealTimeChatClient _realTimeChatClient;
	private readonly EditMessageCommandHandler _handler;

	public EditMessageCommandHandlerTests()
	{
		_userMessageRepository = Substitute.For<IUserMessageRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		_realTimeChatClient = Substitute.For<IRealTimeChatClient>();
		var logger = Substitute.For<ILogger<EditMessageCommandHandler>>();

		_handler = new EditMessageCommandHandler(
			_userMessageRepository,
			_valueProvider,
			_realTimeChatClient,
			logger);
	}

	[Fact]
	public async Task Handle_WhenMessageNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new EditMessageCommand(1, "New Content", Guid.NewGuid());
		_userMessageRepository.GetByIdAsync(command.Id, command.UserId).Returns((UserMessage?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe(
			"Mensagem com o id especificado não existe ou você não tem permissão para editá-la.");
	}

	[Fact]
	public async Task Handle_WhenUserIsNotSender_ShouldThrowNotFoundException()
	{
		// Arrange
		var messageId = 1L;
		var userId = Guid.NewGuid();
		var senderId = Guid.NewGuid();
		var command = new EditMessageCommand(messageId, "New Content", userId);

		var userMessage = CreateUserMessage(messageId, senderId);
		_userMessageRepository.GetByIdAsync(messageId, userId).Returns(userMessage);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe(
			"Mensagem com o id especificado não existe ou você não tem permissão para editá-la.");
	}

	[Fact]
	public async Task Handle_WhenTimeLimitExceeded_ShouldThrowForbiddenException()
	{
		// Arrange
		var messageId = 1L;
		var userId = Guid.NewGuid();
		var command = new EditMessageCommand(messageId, "New Content", userId);

		var messageTime = DateTime.UtcNow.AddMinutes(-10);
		var userMessage = CreateUserMessage(messageId, userId, messageTime);

		_userMessageRepository.GetByIdAsync(messageId, userId).Returns(userMessage);
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível editar a mensagem, o tempo limite foi expirado.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldEditMessageAndNotifyRealTime()
	{
		// Arrange
		var messageId = 1L;
		var userId = Guid.NewGuid();
		var receiverId = Guid.NewGuid();
		var newContent = "Edited Content";
		var command = new EditMessageCommand(messageId, newContent, userId);

		var messageTime = DateTime.UtcNow.AddMinutes(-3);
		var userMessage = CreateUserMessage(messageId, userId, messageTime, receiverId);

		_userMessageRepository.GetByIdAsync(messageId, userId).Returns(userMessage);
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		userMessage.Content.ShouldBe(newContent);
		userMessage.HasBeenEdited.ShouldBeTrue();
		await _userMessageRepository.Received(1).CommitAsync();
		await _realTimeChatClient.Received(1).EditMessage(
			userId,
			receiverId,
			Arg.Is<EditedMessage>(m =>
				m.Id == messageId &&
				m.Content == newContent &&
				m.SenderId == userId &&
				m.ReceiverId == receiverId));
	}

	private static UserMessage CreateUserMessage(
		long messageId,
		Guid senderId,
		DateTime? timeStamp = null,
		Guid? receiverId = null)
	{
		var actualReceiverId = receiverId ?? Guid.NewGuid();

		return new UserMessage
		{
			Id = messageId,
			Sender = new User
			{
				Id = senderId,
				Email = "sender@email.com",
				ReceivesOnlyWhatsAppMessages = true,
				PhoneNumber = "123",
				FullName = "Sender Name",
				Image = "image",
				DefaultAdoptionFormUrl = "url"
			},
			SenderId = senderId,
			Receiver = new User
			{
				Id = actualReceiverId,
				Email = "receiver@email.com",
				ReceivesOnlyWhatsAppMessages = true,
				PhoneNumber = "456",
				FullName = "Receiver Name",
				Image = "image",
				DefaultAdoptionFormUrl = "url"
			},
			ReceiverId = actualReceiverId,
			TimeStampUtc = timeStamp ?? DateTime.UtcNow,
			Content = "Original Content",
			HasBeenEdited = false
		};
	}
}
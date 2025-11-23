using Application.Commands.UserMessages.DeleteMessage;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Providers;
using Application.Common.Interfaces.RealTimeCommunication;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Tests.UserMessages;

public sealed class DeleteMessageCommandHandlerTests
{
	private readonly IUserMessageRepository _userMessageRepository;
	private readonly IValueProvider _valueProvider;
	private readonly IRealTimeChatClient _realTimeChatClient;
	private readonly DeleteMessageCommandHandler _handler;

	public DeleteMessageCommandHandlerTests()
	{
		_userMessageRepository = Substitute.For<IUserMessageRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		_realTimeChatClient = Substitute.For<IRealTimeChatClient>();
		var logger = Substitute.For<ILogger<DeleteMessageCommandHandler>>();

		_handler = new DeleteMessageCommandHandler(
			_userMessageRepository,
			_valueProvider,
			_realTimeChatClient,
			logger);
	}

	[Fact]
	public async Task Handle_WhenMessageNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new DeleteMessageCommand(1, Guid.NewGuid());
		_userMessageRepository.GetByIdAsync(command.MessageId, command.UserId).Returns((UserMessage?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe(
			"Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.");
	}

	[Fact]
	public async Task Handle_WhenUserIsNotSender_ShouldThrowNotFoundException()
	{
		// Arrange
		var messageId = 1L;
		var userId = Guid.NewGuid();
		var senderId = Guid.NewGuid();
		var command = new DeleteMessageCommand(messageId, userId);

		var userMessage = CreateUserMessage(messageId, senderId);
		_userMessageRepository.GetByIdAsync(messageId, userId).Returns(userMessage);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe(
			"Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.");
	}

	[Fact]
	public async Task Handle_WhenTimeLimitExceeded_ShouldThrowForbiddenException()
	{
		// Arrange
		var messageId = 1L;
		var userId = Guid.NewGuid();
		var command = new DeleteMessageCommand(messageId, userId);

		var messageTime = DateTime.UtcNow.AddMinutes(-10);
		var userMessage = CreateUserMessage(messageId, userId, messageTime);

		_userMessageRepository.GetByIdAsync(messageId, userId).Returns(userMessage);
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível excluir a mensagem, o tempo limite foi excedido.");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldDeleteMessageAndNotifyRealTime()
	{
		// Arrange
		var messageId = 1L;
		var userId = Guid.NewGuid();
		var receiverId = Guid.NewGuid();
		var command = new DeleteMessageCommand(messageId, userId);

		var messageTime = DateTime.UtcNow.AddMinutes(-2);
		var userMessage = CreateUserMessage(messageId, userId, messageTime, receiverId);

		_userMessageRepository.GetByIdAsync(messageId, userId).Returns(userMessage);
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		userMessage.HasBeenDeleted.ShouldBeTrue();
		await _userMessageRepository.Received(1).CommitAsync();
		await _realTimeChatClient.Received(1).DeleteMessage(
			senderId: userId,
			receiverId: receiverId,
			Arg.Is<DeletedMessage>(m =>
				m.Id == messageId &&
				m.SenderId == userId &&
				m.ReceiverId == receiverId));
	}

	private static UserMessage CreateUserMessage(
		long messageId,
		Guid senderId,
		DateTime? timeStamp = null,
		Guid? receiverId = null)
	{
		return new UserMessage
		{
			Id = messageId,
			Sender = new User { Id = senderId },
			SenderId = senderId,
			ReceiverId = receiverId ?? Guid.NewGuid(),
			TimeStampUtc = timeStamp ?? DateTime.UtcNow,
			HasBeenDeleted = false
		};
	}
}
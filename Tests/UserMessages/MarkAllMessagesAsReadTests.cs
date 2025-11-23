using Application.Commands.UserMessages.MarkAllMessagesAsRead;
using Application.Common.Interfaces.Entities.UserMessages;
using MediatR;

namespace Tests.UserMessages;

public sealed class MarkAllMessagesAsReadCommandHandlerTests
{
	private readonly IUserMessageRepository _userMessageRepository;
	private readonly MarkAllMessagesAsReadCommandHandler _handler;

	public MarkAllMessagesAsReadCommandHandlerTests()
	{
		_userMessageRepository = Substitute.For<IUserMessageRepository>();

		_handler = new MarkAllMessagesAsReadCommandHandler(_userMessageRepository);
	}

	[Fact]
	public async Task Handle_ShouldMarkAllMessagesAsRead()
	{
		// Arrange
		var senderId = Guid.NewGuid();
		var receiverId = Guid.NewGuid();
		var command = new MarkAllMessagesAsReadCommand(senderId, receiverId);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		await _userMessageRepository.Received(1).ReadAllAsync(senderId, receiverId);
	}
}
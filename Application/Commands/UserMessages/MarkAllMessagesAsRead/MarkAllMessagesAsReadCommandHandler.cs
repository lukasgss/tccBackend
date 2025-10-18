using Application.Common.Interfaces.Entities.UserMessages;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Commands.UserMessages.MarkAllMessagesAsRead;

public record MarkAllMessagesAsReadCommand(Guid SenderId, Guid ReceiverId) : IRequest<Unit>;

public class MarkAllMessagesAsReadCommandHandler : IRequestHandler<MarkAllMessagesAsReadCommand, Unit>
{
    private readonly IUserMessageRepository _userMessageRepository;

    public MarkAllMessagesAsReadCommandHandler(IUserMessageRepository userMessageRepository)
    {
        _userMessageRepository = Guard.Against.Null(userMessageRepository);
    }

    public async Task<Unit> Handle(MarkAllMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        await _userMessageRepository.ReadAllAsync(request.SenderId, request.ReceiverId);

        return Unit.Value;
    }
}
using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Providers;
using Application.Common.Interfaces.RealTimeCommunication;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.UserMessages.DeleteMessage;

[ExcludeFromCodeCoverage]
public sealed record DeleteMessageCommand(long MessageId, Guid UserId) : IRequest<Unit>;

public sealed class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Unit>
{
    private const int MaximumTimeToDeleteMessageInMinutes = 5;

    private readonly IUserMessageRepository _userMessageRepository;
    private readonly IValueProvider _valueProvider;
    private readonly IRealTimeChatClient _realTimeChatClient;
    private readonly ILogger<DeleteMessageCommandHandler> _logger;

    public DeleteMessageCommandHandler(
        IUserMessageRepository userMessageRepository,
        IValueProvider valueProvider,
        IRealTimeChatClient realTimeChatClient,
        ILogger<DeleteMessageCommandHandler> logger)
    {
        _logger = Guard.Against.Null(logger);
        _userMessageRepository = Guard.Against.Null(userMessageRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _realTimeChatClient = Guard.Against.Null(realTimeChatClient);
    }

    public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        UserMessage? dbUserMessage = await _userMessageRepository.GetByIdAsync(request.MessageId, request.UserId);
        if (dbUserMessage is null || dbUserMessage.Sender.Id != request.UserId)
        {
            _logger.LogInformation(
                "Mensagem {MessageId} não existe ou usuário {UserId} não tem permissão para excluí-la",
                request.MessageId, request.UserId);
            throw new NotFoundException(
                "Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.");
        }

        if (_valueProvider.UtcNow().Subtract(dbUserMessage.TimeStampUtc).TotalMinutes >
            MaximumTimeToDeleteMessageInMinutes)
        {
            _logger.LogInformation("Não é possível excluir mensagem {MessageId}, cadastrada em {MessageTimeStamp}",
                request.MessageId, dbUserMessage.TimeStampUtc);
            throw new ForbiddenException(
                "Não é possível excluir a mensagem, o tempo limite foi excedido.");
        }

        await DeleteMessageRealTime(request.UserId, dbUserMessage.ReceiverId, request.MessageId);

        dbUserMessage.HasBeenDeleted = true;
        await _userMessageRepository.CommitAsync();

        return Unit.Value;
    }

    private async Task DeleteMessageRealTime(Guid senderId, Guid receiverId, long messageId)
    {
        DeletedMessage deletedMessage = new()
        {
            Id = messageId,
            SenderId = senderId,
            ReceiverId = receiverId
        };

        await _realTimeChatClient.DeleteMessage(senderId: senderId, receiverId: receiverId, deletedMessage);
    }
}
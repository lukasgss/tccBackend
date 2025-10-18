using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Providers;
using Application.Common.Interfaces.RealTimeCommunication;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.UserMessages.EditMessage;

public record EditMessageCommand(
    long Id,
    string Content,
    Guid UserId
) : IRequest<UserMessageResponse>;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, UserMessageResponse>
{
    private const int MaximumTimeToEditMessageInMinutes = 7;

    private readonly IUserMessageRepository _userMessageRepository;
    private readonly IValueProvider _valueProvider;
    private readonly IRealTimeChatClient _realTimeChatClient;
    private readonly ILogger<EditMessageCommandHandler> _logger;

    public EditMessageCommandHandler(
        IUserMessageRepository userMessageRepository,
        IValueProvider valueProvider,
        IRealTimeChatClient realTimeChatClient, ILogger<EditMessageCommandHandler> logger)
    {
        _logger = Guard.Against.Null(logger);
        _realTimeChatClient = Guard.Against.Null(realTimeChatClient);
        _valueProvider = Guard.Against.Null(valueProvider);
        _userMessageRepository = Guard.Against.Null(userMessageRepository);
    }

    public async Task<UserMessageResponse> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        UserMessage? dbUserMessage = await _userMessageRepository.GetByIdAsync(request.Id, request.UserId);
        if (dbUserMessage is null || dbUserMessage.Sender.Id != request.UserId)
        {
            _logger.LogInformation(
                "Mensagem {MessageId} não existe ou usuário {UserId} não possui permissão para editá-la",
                request.Id, request.UserId);
            throw new NotFoundException(
                "Mensagem com o id especificado não existe ou você não tem permissão para editá-la.");
        }

        if (_valueProvider.UtcNow().Subtract(dbUserMessage.TimeStampUtc).TotalMinutes >
            MaximumTimeToEditMessageInMinutes)
        {
            _logger.LogInformation("Não é possível editar mensagem {MessageId}, cadastrada em {MessageTimeStamp}",
                request.Id, dbUserMessage.TimeStampUtc);
            throw new ForbiddenException("Não é possível editar a mensagem, o tempo limite foi expirado.");
        }

        dbUserMessage.Content = request.Content;
        dbUserMessage.HasBeenEdited = true;
        await _userMessageRepository.CommitAsync();

        await SendEditedMessageRealTime(request.UserId, dbUserMessage.ReceiverId, request.Id, request.Content);

        return dbUserMessage.ToUserMessageResponse();
    }

    private async Task SendEditedMessageRealTime(Guid senderId,
        Guid receiverId,
        long messageId,
        string messageContent)
    {
        EditedMessage editMessage = new()
        {
            Id = messageId,
            Content = messageContent,
            SenderId = senderId,
            ReceiverId = receiverId
        };
        await _realTimeChatClient.EditMessage(senderId, receiverId, editMessage);
    }
}
using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.UserMessages.SendMessage;

[ExcludeFromCodeCoverage]
public sealed record SendUserMessageCommand(
    Guid SenderId,
    Guid ReceiverId,
    string Content) : IRequest<UserMessageResponse>;

public sealed class SendUserMessageCommandHandler : IRequestHandler<SendUserMessageCommand, UserMessageResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMessageRepository _userMessageRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ILogger<SendUserMessageCommandHandler> _logger;

    public SendUserMessageCommandHandler(
        IUserRepository userRepository,
        IUserMessageRepository userMessageRepository,
        IValueProvider valueProvider,
        ILogger<SendUserMessageCommandHandler> logger)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _userMessageRepository = Guard.Against.Null(userMessageRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<UserMessageResponse> Handle(SendUserMessageCommand request, CancellationToken cancellationToken)
    {
        User? receiver = await _userRepository.GetUserByIdAsync(request.ReceiverId);
        if (receiver is null)
        {
            _logger.LogInformation("Destinatário {ReceiverId} não existe", request.ReceiverId);
            throw new NotFoundException("Usuário destinatário não foi encontrado.");
        }

        User? sender = await _userRepository.GetUserByIdAsync(request.SenderId);
        if (sender is null)
        {
            _logger.LogInformation("Remetente {SenderId} não existe", request.SenderId);
            throw new NotFoundException("Usuário remetente não foi encontrado.");
        }

        UserMessage message = new()
        {
            Content = request.Content,
            TimeStampUtc = _valueProvider.UtcNow(),
            HasBeenRead = false,
            HasBeenEdited = false,
            Sender = sender,
            Receiver = receiver
        };

        _userMessageRepository.Add(message);
        await _userMessageRepository.CommitAsync();

        return message.ToUserMessageResponse();
    }
}
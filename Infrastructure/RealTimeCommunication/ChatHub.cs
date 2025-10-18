using System.Security.Claims;
using Application.Commands.UserMessages.SendMessage;
using Application.Common.Exceptions;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.RealTimeCommunication;

public class ChatHub : Hub
{
    private readonly ISender _mediator;

    public ChatHub(ISender mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task SendMessage(string receiverId, string message)
    {
        string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Guid? senderId = null;
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid senderResult))
        {
            senderId = senderResult;
        }

        Guid? guidReceiverId = null;
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(receiverId, out Guid receiverResult))
        {
            guidReceiverId = receiverResult;
        }

        if (senderId is null || guidReceiverId is null)
        {
            throw new BadRequestException("É necessário informar o remetente e o destinatário da mensagem.");
        }

        SendUserMessageCommand command = new(senderId.Value, guidReceiverId.Value, message);
        var messageResponse = await _mediator.Send(command);

        await Clients.Users(userId!, receiverId).SendAsync("ReceiveMessage", messageResponse);
    }
}
using System.ComponentModel.DataAnnotations;
using Application.Commands.UserMessages.DeleteMessage;
using Application.Commands.UserMessages.EditMessage;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Pagination;
using Application.Queries.UserMessages.GetAllUsersConversations;
using Application.Queries.UserMessages.GetMessagesFromUser;
using Application.Queries.UserMessages.GetNotificationsAmount;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/user-messages")]
public class UserMessageController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public UserMessageController(IUserAuthorizationService userAuthorizationService, ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet]
    public async Task<IList<UserConversation>> GetAllUserConversations()
    {
        Guid currentUserId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetAllUserConversationsQuery query = new(currentUserId);
        return await _mediator.Send(query);
    }

    [HttpGet("notifications")]
    public async Task<NotificationAmountResponse> GetNotifications()
    {
        Guid currentUserId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetNotificationsAmountQuery query = new(currentUserId);
        return await _mediator.Send(query);
    }

    [HttpGet("specific")]
    public async Task<PaginatedEntity<UserMessageResponse>> GetAllMessages(
        [Required] Guid otherUserId,
        int pageNumber = 1,
        int pageSize = PagedList<UserMessage>.MaxPageSize)
    {
        Guid currentUserId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetMessagesFromUserQuery query = new(currentUserId, otherUserId, pageNumber, pageSize);
        return await _mediator.Send(query);
    }

    [HttpPut("{messageId:long}")]
    public async Task<ActionResult<UserMessageResponse>> EditAsync(
        long messageId, EditMessageRequest message)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        EditMessageCommand command = new(messageId, message.Content, userId);
        return await _mediator.Send(command);
    }

    [HttpDelete("{messageId:long}")]
    public async Task<ActionResult> DeleteAsync(long messageId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        DeleteMessageCommand command = new(messageId, userId);
        await _mediator.Send(command);
        return NoContent();
    }
}
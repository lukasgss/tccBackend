using Application.Commands.AdoptionAlertNotifications.ReadAllNotifications;
using Application.Commands.AdoptionAlertNotifications.ReadNotification;
using Application.Common.Interfaces.Authorization;
using Application.Queries.AdoptionAlertNotifications.GetNotifications;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/adoption-notifications")]
public class AdoptionNotificationsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public AdoptionNotificationsController(ISender mediator, IUserAuthorizationService userAuthorizationService)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [Authorize]
    [HttpGet]
    public async Task<List<AdoptionAlertNotificationResponse>> GetNotifications()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetAdoptionAlertNotificationsQuery query = new(userId);
        return await _mediator.Send(query);
    }

    [Authorize]
    [HttpPost("read/{notificationId:long}")]
    public async Task<ActionResult> ReadNotification(long notificationId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        ReadAdoptionAlertNotificationCommand command = new(notificationId, userId);
        await _mediator.Send(command);

        return Ok();
    }

    [Authorize]
    [HttpPost("read")]
    public async Task<ActionResult> ReadAllNotifications()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        ReadAllAdoptionNotificationsCommand command = new(userId);
        await _mediator.Send(command);

        return Ok();
    }
}
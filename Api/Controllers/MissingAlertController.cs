using Application.Commands.MissingAlerts.Create;
using Application.Commands.MissingAlerts.Delete;
using Application.Commands.MissingAlerts.Edit;
using Application.Commands.MissingAlerts.ToggleFound;
using Application.Common.ApplicationConstants;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Queries.MissingAlerts.GetById;
using Application.Queries.MissingAlerts.ListMissingAlerts;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/missing-alerts")]
public class MissingAlertController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public MissingAlertController(
        IUserAuthorizationService userAuthorizationService,
        ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet]
    public async Task<PaginatedEntity<MissingAlertResponse>> ListMissingAlerts(
        [FromQuery] MissingAlertFilters filters,
        int page = 1,
        int pageSize = Constants.DefaultPageSize)
    {
        ListMissingAlertsQuery query = new(filters, page, pageSize);
        return await _mediator.Send(query);
    }

    [HttpGet("{alertId:guid}", Name = "GetMissingAlertById")]
    public async Task<MissingAlertResponseWithGeoLocation> GetMissingAlertById(Guid alertId)
    {
        GetMissingAlertByIdQuery query = new(alertId);
        return await _mediator.Send(query);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<MissingAlertResponse>> Create(
        [FromForm] CreateMissingAlertRequest request)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);
        CreateMissingAlertCommand command = new(
            request.State,
            request.City,
            request.Neighborhood,
            request.Description,
            request.Pet,
            userId);

        MissingAlertResponse createdMissingAlert = await _mediator.Send(command);

        return new CreatedAtRouteResult(
            nameof(GetMissingAlertById),
            new { alertId = createdMissingAlert.Id },
            createdMissingAlert);
    }

    [Authorize]
    [HttpPost("find/{alertId:guid}")]
    public async Task<ActionResult> ToggleFound(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        ToggleFoundMissingAlertCommand missingAlertCommand = new(alertId, userId);
        await _mediator.Send(missingAlertCommand);

        return NoContent();
    }

    [Authorize]
    [HttpPut("{alertId:guid}")]
    public async Task<MissingAlertResponse> Edit(EditMissingAlertRequest request, Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        EditMissingAlertCommand command = new(alertId,
            request.LastSeenLocationLatitude,
            request.LastSeenLocationLongitude,
            request.Description,
            request.PetId,
            userId);

        return await _mediator.Send(command);
    }

    [Authorize]
    [HttpDelete("{alertId:guid}")]
    public async Task<ActionResult> Delete(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        DeleteMissingAlertCommand command = new(alertId, userId);
        await _mediator.Send(command);

        return NoContent();
    }
}
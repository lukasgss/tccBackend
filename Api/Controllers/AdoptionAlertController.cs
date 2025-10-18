using Application.Commands.AdoptionAlerts.CreateAdoptionAlert;
using Application.Commands.AdoptionAlerts.Delete;
using Application.Commands.AdoptionAlerts.ToggleAdoption;
using Application.Commands.AdoptionAlerts.Update;
using Application.Common.ApplicationConstants;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Queries.AdoptionAlerts.GetById;
using Application.Queries.AdoptionAlerts.GetSharingAlertPoster;
using Application.Queries.AdoptionAlerts.GetSuggesteds;
using Application.Queries.AdoptionAlerts.GetUserCreatedAlerts;
using Application.Queries.AdoptionAlerts.GetUserSavedAlerts;
using Application.Queries.AdoptionAlerts.ListAlerts;
using Application.Queries.AdoptionFavorites.GetById;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/adoption-alerts")]
public class AdoptionAlertController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public AdoptionAlertController(
        IUserAuthorizationService userAuthorizationService,
        ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("{alertId:guid}", Name = "GetAdoptionAlertById")]
    public async Task<AdoptionAlertResponseWithGeoLocation> GetAdoptionAlertById(Guid alertId)
    {
        Guid? userId = _userAuthorizationService.GetPotentialUserId(User);

        GetAdoptionAlertByIdQuery query = new(alertId, userId);
        return await _mediator.Send(query);
    }

    [HttpGet]
    public async Task<PaginatedEntity<AdoptionAlertListingResponse>> ListAdoptionAlerts(
        [FromQuery] AdoptionAlertFilters filters,
        int page = 1,
        int pageSize = Constants.DefaultPageSize)
    {
        Guid? userId = _userAuthorizationService.GetPotentialUserId(User);

        ListAdoptionAlertsQuery query = new(filters, userId, page, pageSize);
        return await _mediator.Send(query);
    }

    [HttpGet("suggested")]
    public async Task<IList<SuggestedAlertsQueryResponse>> GetSuggestedAlerts(double? latitude, double? longitude)
    {
        GetSuggestedsBasedOnLocationQuery query = new(latitude, longitude);
        return await _mediator.Send(query);
    }

    [HttpGet("sharing-poster")]
    public async Task<IActionResult> GetSharingPoster(Guid alertId)
    {
        GetSharingAlertPosterQuery query = new(alertId);
        SharingAlertPosterResponse posterResponse = await _mediator.Send(query);

        return File(
            posterResponse.PosterFile,
            "application/pdf",
            $"adocao_{posterResponse.PetName.ToLowerInvariant()}.pdf");
    }

    [Authorize]
    [HttpGet("created")]
    public async Task<IList<AdoptionAlertListingResponse>> GetUserCreatedAlerts()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetUserCreatedAlertsQuery query = new(userId);
        return await _mediator.Send(query);
    }

    [Authorize]
    [HttpGet("saved")]
    public async Task<IList<AdoptionAlertListingResponse>> GetUserSavedAlerts()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetUserSavedAlertsQuery query = new(userId);
        return await _mediator.Send(query);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AdoptionAlertResponse>> Create([FromForm] CreateAdoptionAlertRequest request)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        CreateAdoptionAlertCommand command = new(
            userId,
            request.AdoptionRestrictions,
            request.Neighborhood,
            request.State,
            request.City,
            request.Description,
            request.Pet,
            request.ForceCreationWithNotFoundCoordinates,
            request.AdoptionForm,
            request.ShouldUseDefaultAdoptionForm);

        AdoptionAlertResponse adoptionAlertResponse = await _mediator.Send(command);

        return new CreatedAtRouteResult(
            nameof(GetAdoptionAlertById),
            new { alertId = adoptionAlertResponse.Id },
            adoptionAlertResponse);
    }

    [Authorize]
    [HttpPost("adopt/{alertId:guid}")]
    public async Task<AdoptionAlertResponse> ToggleAdoption(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        ToggleAdoptionCommand command = new(alertId, userId);
        return await _mediator.Send(command);
    }

    [Authorize]
    [HttpPut("{alertId:guid}")]
    public async Task<AdoptionAlertResponse> Edit([FromForm] UpdateAdoptionAlertRequest request, Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        UpdateAdoptionAlertCommand command = new(
            alertId,
            request.Neighborhood,
            request.State,
            request.City,
            request.Description,
            request.Pet,
            userId,
            request.AdoptionRestrictions,
            request.ForceCreationWithNotFoundCoordinates,
            request.AdoptionForm,
            request.ExistingAdoptionFormUrl,
            request.ShouldUseDefaultAdoptionForm);
        return await _mediator.Send(command);
    }

    [Authorize]
    [HttpDelete("{alertId:guid}")]
    public async Task<ActionResult> Delete(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        DeleteAdoptionCommand command = new(alertId, userId);

        await _mediator.Send(command);
        return NoContent();
    }
}
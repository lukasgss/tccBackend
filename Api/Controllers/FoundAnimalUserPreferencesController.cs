using Application.Commands.FoundAlertPreferences;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Queries.FoundAlertsPreferences;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/user-preferences/found-animals")]
public class FoundAnimalUserPreferencesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public FoundAnimalUserPreferencesController(IUserAuthorizationService userAuthorizationService, ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [Authorize]
    [HttpGet(Name = "GetFoundAnimalUserPreferences")]
    public async Task<UserPreferencesResponse> GetFoundAnimalUserPreferences()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetFoundAnimalUserPreferencesQuery query = new(userId);
        return await _mediator.Send(query);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<UserPreferencesResponse>> CreatePreferences(UserPreferencesRequest request)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        AssignFoundAlertUserPreferencesCommand command = new(userId,
            request.FoundLocationLatitude,
            request.FoundLocationLongitude,
            request.RadiusDistanceInKm,
            request.Genders,
            request.Ages,
            request.Sizes,
            request.SpeciesIds,
            request.BreedIds,
            request.ColorIds);

        UserPreferencesResponse userPreferences = await _mediator.Send(command);

        return new CreatedAtRouteResult(nameof(GetFoundAnimalUserPreferences), null, userPreferences);
    }
}
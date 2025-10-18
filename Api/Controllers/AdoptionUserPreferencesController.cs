using Application.Commands.AdoptionAlertPreferences.Assign;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Queries.AdoptionUserPreferences.GetUserPreferences;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/user-preferences/adoptions")]
public class AdoptionUserPreferencesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public AdoptionUserPreferencesController(IUserAuthorizationService userAuthorizationService, ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [Authorize]
    [HttpGet(Name = "GetAdoptionUserPreferences")]
    public async Task<UserPreferencesResponse?> GetAdoptionUserPreferences()
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetAdoptionPreferencesQuery query = new(userId);
        return await _mediator.Send(query);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<UserPreferencesResponse>> SavePreferences(UserPreferencesRequest request)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        AssignAdoptionUserPreferencesCommand command = new(userId,
            request.FoundLocationLongitude,
            request.FoundLocationLongitude,
            request.RadiusDistanceInKm,
            request.Genders,
            request.Ages,
            request.Sizes,
            request.SpeciesIds,
            request.BreedIds,
            request.ColorIds);

        UserPreferencesResponse userPreferences = await _mediator.Send(command);
        return new CreatedAtRouteResult(nameof(GetAdoptionUserPreferences), null, userPreferences);
    }
}
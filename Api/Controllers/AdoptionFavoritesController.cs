using Application.Commands.AdoptionFavorites.ToggleFavorite;
using Application.Common.ApplicationConstants;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Queries.AdoptionFavorites.GetAll;
using Application.Queries.AdoptionFavorites.GetById;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/favorite/adoptions")]
public class AdoptionFavoritesController : ControllerBase
{
    private readonly IUserAuthorizationService _userAuthorizationService;
    private readonly ISender _mediator;

    public AdoptionFavoritesController(IUserAuthorizationService userAuthorizationService, ISender mediator)
    {
        _userAuthorizationService = Guard.Against.Null(userAuthorizationService);
        _mediator = Guard.Against.Null(mediator);
    }

    [HttpGet("{favoriteId:guid}", Name = "GetFavoriteByIdAsync")]
    public async Task<AdoptionFavoriteResponse> GetFavoriteByIdAsync(Guid favoriteId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetAdoptionFavoriteByIdQuery query = new(favoriteId, userId);
        return await _mediator.Send(query);
    }

    [HttpGet]
    public async Task<PaginatedEntity<AdoptionFavoriteResponse>> GetAllFavorites(
        int page = 1, int pageSize = Constants.DefaultPageSize)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        GetAllAdoptionFavoritesQuery query = new(userId, page, pageSize);
        return await _mediator.Send(query);
    }

    [HttpPost("{alertId:guid}")]
    public async Task<ActionResult<AdoptionFavoriteResponse>> ToggleFavorite(Guid alertId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        ToggleFavoriteCommand command = new(userId, alertId);

        AdoptionFavoriteResponse adoptionFavorite = await _mediator.Send(command);

        return new CreatedAtRouteResult(
            nameof(GetFavoriteByIdAsync),
            new { favoriteId = adoptionFavorite.Id },
            adoptionFavorite);
    }
}
using Application.Commands.FoundAlertFavorites.ToggleFavorite;
using Application.Common.ApplicationConstants;
using Application.Common.DTOs;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Queries.AdoptionFavorites.GetAll;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/favorites/found")]
public sealed class FoundAnimalFavoritesController : ControllerBase
{
	private readonly IUserAuthorizationService _userAuthorizationService;
	private readonly ISender _mediator;

	public FoundAnimalFavoritesController(IUserAuthorizationService userAuthorizationService, ISender mediator)
	{
		_userAuthorizationService = Guard.Against.Null(userAuthorizationService);
		_mediator = Guard.Against.Null(mediator);
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
	public async Task<ActionResult<FoundAnimalFavoriteResponse>> ToggleFavorite(Guid alertId)
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		ToggleFoundAlertFavoriteCommand command = new(userId, alertId);

		FoundAnimalFavoriteResponse foundFavorite = await _mediator.Send(command);

		return new ObjectResult(foundFavorite)
		{
			StatusCode = StatusCodes.Status201Created
		};
	}
}
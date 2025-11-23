using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Persistence;
using Application.Common.Pagination;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.AdoptionFavorites.GetAll;

[ExcludeFromCodeCoverage]
public sealed record GetAllAdoptionFavoritesQuery(Guid UserId, int Page, int PageSize)
    : IRequest<PaginatedEntity<AdoptionFavoriteResponse>>;

public class
    GetAllAdoptionFavoritesQueryHandler : IRequestHandler<GetAllAdoptionFavoritesQuery,
    PaginatedEntity<AdoptionFavoriteResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetAllAdoptionFavoritesQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<PaginatedEntity<AdoptionFavoriteResponse>> Handle(GetAllAdoptionFavoritesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.AdoptionFavorites
            .AsNoTracking()
            .Include(favorite => favorite.AdoptionAlert)
            .ThenInclude(alert => alert.Pet)
            .ThenInclude(pet => pet.Images)
            .Where(favorite => favorite.User.Id == request.UserId)
            .Select(favorite => new AdoptionFavoriteResponse(
                    favorite.Id,
                    new SimplifiedAdoptionAlertResponse(
                        favorite.AdoptionAlertId,
                        favorite.AdoptionAlert.AdoptionRestrictions,
                        favorite.AdoptionAlert.Location != null
                            ? favorite.AdoptionAlert.Location.Y
                            : null,
                        favorite.AdoptionAlert.Location != null
                            ? favorite.AdoptionAlert.Location.X
                            : null,
                        favorite.AdoptionAlert.Description,
                        favorite.AdoptionAlert.RegistrationDate,
                        favorite.AdoptionAlert.AdoptionDate,
                        favorite.AdoptionAlert.Pet.ToSimplifiedPetResponse()
                    )
                )
            );

        var adoptionFavorites =
            await PagedList<AdoptionFavoriteResponse>.ToPagedListAsync(query, request.Page, request.PageSize);

        return adoptionFavorites.ToAlertFavoritesResponse();
    }
}
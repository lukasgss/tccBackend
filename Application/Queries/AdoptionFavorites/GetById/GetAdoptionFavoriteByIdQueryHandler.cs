using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.AdoptionFavorites.GetById;

[ExcludeFromCodeCoverage]
public record GetAdoptionFavoriteByIdQuery(Guid FavoriteId, Guid UserId) : IRequest<AdoptionFavoriteResponse>;

public class
    GetAdoptionFavoriteByIdQueryHandler : IRequestHandler<GetAdoptionFavoriteByIdQuery, AdoptionFavoriteResponse>
{
    private readonly IAppDbContext _dbContext;

    public GetAdoptionFavoriteByIdQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<AdoptionFavoriteResponse> Handle(GetAdoptionFavoriteByIdQuery request,
        CancellationToken cancellationToken)
    {
        AdoptionFavoriteResponse? adoptionFavorite = await _dbContext.AdoptionFavorites
            .AsNoTracking()
            .Include(favorite => favorite.AdoptionAlert)
            .ThenInclude(alert => alert.Pet)
            .ThenInclude(pet => pet.Images)
            .Where(favorite => favorite.Id == request.FavoriteId && favorite.UserId == request.UserId)
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
            )
            .SingleOrDefaultAsync(cancellationToken);

        if (adoptionFavorite is null)
        {
            throw new NotFoundException("Não foi possível encontrar o item favoritado.");
        }

        return adoptionFavorite;
    }
}
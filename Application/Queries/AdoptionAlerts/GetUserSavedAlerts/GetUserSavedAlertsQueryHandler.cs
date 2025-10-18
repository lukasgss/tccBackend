using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.AdoptionAlerts.GetUserSavedAlerts;

public record GetUserSavedAlertsQuery(Guid UserId) : IRequest<IList<AdoptionAlertListingResponse>>;

public class
    GetUserSavedAlertsQueryHandler : IRequestHandler<GetUserSavedAlertsQuery, IList<AdoptionAlertListingResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetUserSavedAlertsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<AdoptionAlertListingResponse>> Handle(GetUserSavedAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Select(user => new
            {
                user.Id
            })
            .SingleOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        var savedAlerts = await _dbContext.AdoptionFavorites
            .AsNoTracking()
            .Where(favorite => favorite.UserId == request.UserId)
            .Select(favorite => new AdoptionAlertListing()
            {
                Id = favorite.AdoptionAlert.Id,
                Location = favorite.AdoptionAlert.Location,
                Description = favorite.AdoptionAlert.Description,
                RegistrationDate = favorite.AdoptionAlert.RegistrationDate,
                AdoptionDate = favorite.AdoptionAlert.AdoptionDate,
                AdoptionRestrictions = favorite.AdoptionAlert.AdoptionRestrictions,
                Pet = new Pet()
                {
                    Id = favorite.AdoptionAlert.Pet.Id,
                    Name = favorite.AdoptionAlert.Pet.Name,
                    Species = favorite.AdoptionAlert.Pet.Species,
                    Breed = favorite.AdoptionAlert.Pet.Breed,
                    Images = favorite.AdoptionAlert.Pet.Images,
                    Age = favorite.AdoptionAlert.Pet.Age,
                    Size = favorite.AdoptionAlert.Pet.Size
                },
                User = favorite.AdoptionAlert.User,
                IsFavorite = true
            })
            .ToListAsync(cancellationToken);

        return savedAlerts.ToAdoptionAlertListingResponse();
    }
}
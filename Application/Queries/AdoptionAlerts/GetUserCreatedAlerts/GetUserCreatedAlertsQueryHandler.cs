using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.AdoptionAlerts.GetUserCreatedAlerts;

public record GetUserCreatedAlertsQuery(Guid UserId) : IRequest<IList<AdoptionAlertListingResponse>>;

public class
    GetUserCreatedAlertsQueryHandler : IRequestHandler<GetUserCreatedAlertsQuery,
    IList<AdoptionAlertListingResponse>>
{
    private readonly IAppDbContext _dbContext;

    public GetUserCreatedAlertsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<AdoptionAlertListingResponse>> Handle(GetUserCreatedAlertsQuery request,
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

        var savedAlerts = await _dbContext.AdoptionAlerts
            .AsNoTracking()
            .Where(alert => alert.UserId == request.UserId)
            .Select(alert => new AdoptionAlertListing()
            {
                Id = alert.Id,
                Location = alert.Location,
                Description = alert.Description,
                RegistrationDate = alert.RegistrationDate,
                AdoptionDate = alert.AdoptionDate,
                AdoptionRestrictions = alert.AdoptionRestrictions,
                Pet = new Pet()
                {
                    Id = alert.Pet.Id,
                    Name = alert.Pet.Name,
                    Gender = alert.Pet.Gender,
                    Age = alert.Pet.Age,
                    Size = alert.Pet.Size,
                    UserId = alert.Pet.UserId,
                    BreedId = alert.Pet.BreedId,
                    SpeciesId = alert.Pet.SpeciesId,
                    Breed = alert.Pet.Breed,
                    Species = alert.Pet.Species,
                    Colors = alert.Pet.Colors,
                    Images = alert.Pet.Images
                },
                User = new User()
                {
                    Id = alert.User.Id,
                    FullName = alert.User.FullName,
                    Image = alert.User.Image
                },
                IsFavorite = alert.AdoptionFavorites.Any(favorite => favorite.UserId == request.UserId)
            })
            .OrderByDescending(alert => alert.IsFavorite)
            .ToListAsync(cancellationToken);

        return savedAlerts.ToAdoptionAlertListingResponse();
    }
}
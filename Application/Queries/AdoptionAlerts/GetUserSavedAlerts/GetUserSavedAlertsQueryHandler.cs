using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Persistence;
using Application.Queries.Users.Common;
using Ardalis.GuardClauses;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.AdoptionAlerts.GetUserSavedAlerts;

[ExcludeFromCodeCoverage]
public record GetUserSavedAlertsQuery(Guid UserId) : IRequest<SavedAlertsResponse>;

public sealed class GetUserSavedAlertsQueryHandler : IRequestHandler<GetUserSavedAlertsQuery, SavedAlertsResponse>
{
    private readonly IAppDbContext _dbContext;

    public GetUserSavedAlertsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<SavedAlertsResponse> Handle(GetUserSavedAlertsQuery request,
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

        var savedAdoptionAlerts = await _dbContext.AdoptionFavorites
            .AsNoTracking()
            .Where(favorite => favorite.UserId == request.UserId)
            .Select(alert => new SavedAdoptionListingResponse
            {
                Id = alert.AdoptionAlert.Id,
                Description = alert.AdoptionAlert.Description,
                RegistrationDate = alert.AdoptionAlert.RegistrationDate,
                AdoptionDate = alert.AdoptionAlert.AdoptionDate,
                AdoptionRestrictions = alert.AdoptionAlert.AdoptionRestrictions,
                Pet = new ExtraSimplifiedPetResponse(
                    Id: alert.Id,
                    Name: alert.AdoptionAlert.Pet.Name,
                    Age: new AgeResponse(alert.AdoptionAlert.Pet.Age, alert.AdoptionAlert.Pet.Age.ToString()),
                    Breed: new BreedResponse(
                        Id: alert.AdoptionAlert.Pet.Breed.Id,
                        Name: alert.AdoptionAlert.Pet.Breed.Name
                    ),
                    Gender: new GenderResponse(
                        id: alert.AdoptionAlert.Pet.Gender,
                        name: alert.AdoptionAlert.Pet.Gender.ToString()
                    ),
                    Images: alert.AdoptionAlert.Pet.Images.Select(i => i.ImageUrl).ToList()
                ),
                Owner = new UserDataResponse(
                    Id: alert.User.Id,
                    Image: alert.User.Image,
                    FullName: alert.User.FullName,
                    Email: alert.User.Email ?? string.Empty,
                    PhoneNumber: alert.User.PhoneNumber,
                    OnlyWhatsAppMessages: alert.User.ReceivesOnlyWhatsAppMessages,
                    DefaultAdoptionFormUrl: alert.User.DefaultAdoptionFormUrl
                ),
                IsFavorite = alert.AdoptionAlert.AdoptionFavorites.Any(favorite => favorite.UserId == request.UserId)
            })
            .ToListAsync(cancellationToken);

        var savedFoundAlerts = await _dbContext.FoundAnimalFavorites
            .AsNoTracking()
            .Where(alert => alert.UserId == request.UserId)
            .Select(alert => new FoundAnimalAlertResponse(
                Id: alert.FoundAnimalAlert.Id,
                Name: alert.FoundAnimalAlert.Name,
                Description: alert.FoundAnimalAlert.Description,
                FoundLocationLatitude: alert.FoundAnimalAlert.Location.Y,
                FoundLocationLongitude: alert.FoundAnimalAlert.Location.X,
                RegistrationDate: alert.FoundAnimalAlert.RegistrationDate,
                RecoveryDate: alert.FoundAnimalAlert.RecoveryDate,
                Pet: new ExtraSimplifiedPetResponse(
                    Id: alert.Id,
                    Name: alert.FoundAnimalAlert.Name ?? string.Empty,
                    Age: new AgeResponse(alert.FoundAnimalAlert.Age, alert.FoundAnimalAlert.Age.ToString()),
                    Breed: new BreedResponse(
                        Id: alert.FoundAnimalAlert.Breed != null ? alert.FoundAnimalAlert.Breed.Id : 0,
                        Name: alert.FoundAnimalAlert.Breed != null ? alert.FoundAnimalAlert.Breed.Name : "Desconhecido"
                    ),
                    Gender: new GenderResponse(
                        id: alert.FoundAnimalAlert.Gender != null
                            ? alert.FoundAnimalAlert.Gender.Value
                            : Gender.Desconhecido,
                        name: alert.FoundAnimalAlert.Gender != null
                            ? alert.FoundAnimalAlert.Gender.Value.ToString()
                            : "Desconhecido"
                    ),
                    Images: alert.FoundAnimalAlert.Images.Select(i => i.ImageUrl).ToList()
                ),
                Owner: new UserDataResponse(
                    Id: alert.User.Id,
                    Image: alert.User.Image,
                    FullName: alert.User.FullName,
                    Email: alert.User.Email ?? string.Empty,
                    PhoneNumber: alert.User.PhoneNumber,
                    OnlyWhatsAppMessages: alert.User.ReceivesOnlyWhatsAppMessages,
                    DefaultAdoptionFormUrl: alert.User.DefaultAdoptionFormUrl
                )
            ))
            .ToListAsync(cancellationToken);

        return new SavedAlertsResponse(AdoptionAlerts: savedAdoptionAlerts, FoundAnimalAlerts: savedFoundAlerts);
    }
}
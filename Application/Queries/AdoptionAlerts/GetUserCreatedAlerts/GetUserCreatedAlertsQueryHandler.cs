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

namespace Application.Queries.AdoptionAlerts.GetUserCreatedAlerts;

[ExcludeFromCodeCoverage]
public record GetUserCreatedAlertsQuery(Guid UserId) : IRequest<CreatedAlertsResponse>;

public sealed class GetUserCreatedAlertsQueryHandler : IRequestHandler<GetUserCreatedAlertsQuery,
    CreatedAlertsResponse>
{
    private readonly IAppDbContext _dbContext;

    public GetUserCreatedAlertsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<CreatedAlertsResponse> Handle(GetUserCreatedAlertsQuery request,
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

        var createdAdoptionAlerts = await _dbContext.AdoptionAlerts
            .AsNoTracking()
            .Where(alert => alert.UserId == request.UserId)
            .Select(alert => new CreatedAdoptionListingResponse
            {
                Id = alert.Id,
                Description = alert.Description,
                RegistrationDate = alert.RegistrationDate,
                AdoptionDate = alert.AdoptionDate,
                AdoptionRestrictions = alert.AdoptionRestrictions,
                Pet = new ExtraSimplifiedPetResponse(
                    Id: alert.Id,
                    Name: alert.Pet.Name ?? string.Empty,
                    Age: new AgeResponse(alert.Pet.Age, alert.Pet.Age.ToString()),
                    Breed: new BreedResponse(
                        Id: alert.Pet.Breed.Id,
                        Name: alert.Pet.Breed.Name
                    ),
                    Gender: new GenderResponse(
                        id: alert.Pet.Gender,
                        name: alert.Pet.Gender.ToString()
                    ),
                    Images: alert.Pet.Images.Select(i => i.ImageUrl).ToList()
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
                IsFavorite = alert.AdoptionFavorites.Any(favorite => favorite.UserId == request.UserId)
            })
            .ToListAsync(cancellationToken);

        var createdFoundAlerts = await _dbContext.FoundAnimalAlerts
            .AsNoTracking()
            .Where(alert => alert.UserId == request.UserId)
            .Select(alert => new FoundAnimalAlertResponse(
                Id: alert.Id,
                Name: alert.Name,
                Description: alert.Description,
                FoundLocationLatitude: alert.Location.Y,
                FoundLocationLongitude: alert.Location.X,
                RegistrationDate: alert.RegistrationDate,
                RecoveryDate: alert.RecoveryDate,
                Pet: new ExtraSimplifiedPetResponse(
                    Id: alert.Id,
                    Name: alert.Name ?? string.Empty,
                    Age: new AgeResponse(alert.Age, alert.Age.ToString()),
                    Breed: new BreedResponse(
                        Id: alert.Breed != null ? alert.Breed.Id : 0,
                        Name: alert.Breed != null ? alert.Breed.Name : "Desconhecido"
                    ),
                    Gender: new GenderResponse(
                        id: alert.Gender != null ? alert.Gender.Value : Gender.Desconhecido,
                        name: alert.Gender != null ? alert.Gender.Value.ToString() : "Desconhecido"
                    ),
                    Images: alert.Images.Select(i => i.ImageUrl).ToList()
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

        return new CreatedAlertsResponse(createdAdoptionAlerts, createdFoundAlerts);
    }
}
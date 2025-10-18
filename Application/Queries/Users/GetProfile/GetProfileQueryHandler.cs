using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.Users.GetProfile;

public record GetProfileQuery(Guid UserId) : IRequest<UserProfileResponse>;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    private readonly IAppDbContext _dbContext;

    public GetProfileQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        UserProfileResponse? user = await _dbContext.Users
            .Where(user => user.Id == request.UserId)
            .Select(user => new UserProfileResponse(
                user.Id,
                user.Image,
                user.FullName,
                user.Email!,
                user.PhoneNumber,
                user.ReceivesOnlyWhatsAppMessages,
                user.AdoptionAlerts.Select(alert => new AdoptionAlertProfileListing(
                    alert.Id,
                    alert.AdoptionRestrictions,
                    alert.Location != null ? alert.Location.Y : null,
                    alert.Location != null ? alert.Location.X : null,
                    alert.Description,
                    user.AdoptionFavorites.Any(favorite => favorite.AdoptionAlert.Id == alert.Id),
                    alert.RegistrationDate,
                    alert.AdoptionDate,
                    new ExtraSimplifiedPetResponse(
                        alert.Pet.Id,
                        alert.Pet.Name,
                        alert.Pet.Age.ToAgeResponse(),
                        alert.Pet.Breed.ToBreedResponse(),
                        alert.Pet.Gender.ToGenderResponse(),
                        alert.Pet.Images.Select(img => img.ImageUrl).ToList()
                    )
                )).ToList()
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Usuário especificado não existe.");
        }

        return user;
    }
}
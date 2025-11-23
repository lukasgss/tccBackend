using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Persistence;
using Application.Queries.Users.Common;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.AdoptionUserPreferences.GetUserPreferences;

[ExcludeFromCodeCoverage]
public record GetAdoptionPreferencesQuery(Guid UserId) : IRequest<UserPreferencesResponse?>;

public class GetAdoptionPreferencesQueryHandler
    : IRequestHandler<GetAdoptionPreferencesQuery, UserPreferencesResponse?>
{
    private readonly IAppDbContext _dbContext;

    public GetAdoptionPreferencesQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<UserPreferencesResponse?> Handle(GetAdoptionPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.AdoptionUserPreferences
            .AsNoTracking()
            .Where(preferences => preferences.User.Id == request.UserId)
            .Select(preference => new UserPreferencesResponse(
                    preference.Id,
                    preference.Location != null ? preference.Location.Y : null,
                    preference.Location != null ? preference.Location.X : null,
                    preference.RadiusDistanceInKm,
                    preference.Genders.ToListOfGenderResponse(),
                    preference.Ages.ToListOfAgeResponse(),
                    preference.Sizes.ToListOfSizeResponse(),
                    preference.Species.ToListOfSpeciesResponse(),
                    preference.Breeds.ToListOfBreedResponse(),
                    preference.Colors.ToListOfColorResponse(),
                    new UserDataResponse(
                        preference.User.Id,
                        preference.User.Image,
                        preference.User.FullName,
                        preference.User.Email!,
                        preference.User.PhoneNumber,
                        preference.User.ReceivesOnlyWhatsAppMessages,
                        preference.User.DefaultAdoptionFormUrl)
                )
            )
            .SingleOrDefaultAsync(cancellationToken);
    }
}
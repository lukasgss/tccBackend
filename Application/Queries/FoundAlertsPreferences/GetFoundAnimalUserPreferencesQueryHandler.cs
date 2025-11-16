using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.FoundAlertsPreferences;

public record GetFoundAnimalUserPreferencesQuery(Guid UserId) : IRequest<UserPreferencesResponse>;

public class GetFoundAnimalUserPreferencesQueryHandler
    : IRequestHandler<GetFoundAnimalUserPreferencesQuery, UserPreferencesResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetFoundAnimalUserPreferencesQueryHandler> _logger;

    public GetFoundAnimalUserPreferencesQueryHandler(
        IAppDbContext dbContext,
        ILogger<GetFoundAnimalUserPreferencesQueryHandler> logger)
    {
        _dbContext = Guard.Against.Null(dbContext);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<UserPreferencesResponse> Handle(GetFoundAnimalUserPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        UserPreferencesResponse? userPreferences = await _dbContext.FoundAnimalUserPreferences
            .AsNoTracking()
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
                    preference.User.ToUserDataResponse()
                )
            )
            .SingleOrDefaultAsync(preferences => preferences.User.Id == request.UserId, cancellationToken);
        if (userPreferences is null)
        {
            _logger.LogInformation("Ainda não foram definidas preferências pro usuário {UserId}", request.UserId);
            throw new NotFoundException(
                "Ainda não foram definidas preferências desse tipo de alerta para este usuário.");
        }

        return userPreferences;
    }
}
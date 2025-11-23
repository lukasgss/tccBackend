using System.Diagnostics.CodeAnalysis;
using Application.Common.Calculators;
using Application.Common.Converters;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Persistence;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.AdoptionAlerts.GetSuggesteds;

[ExcludeFromCodeCoverage]
public record GetSuggestedsBasedOnLocationQuery(
    double? Latitude = 0,
    double? Longitude = 0)
    : IRequest<IList<SuggestedAlertsQueryResponse>>;

[ExcludeFromCodeCoverage]
public record SuggestedAlertsQueryResponse(
    Guid Id,
    List<string> AdoptionRestrictions,
    double? LocationLatitude,
    double? LocationLongitude,
    string? Description,
    DateTime RegistrationDate,
    DateOnly? AdoptionDate,
    ExtraSimplifiedPetResponse Pet);

public class GetSuggestedsBasedOnLocationQueryHandler
    : IRequestHandler<GetSuggestedsBasedOnLocationQuery, IList<SuggestedAlertsQueryResponse>>
{
    private const int AmountOfSuggestedAlertsReturned = 4;

    private readonly IAppDbContext _dbContext;

    public GetSuggestedsBasedOnLocationQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public async Task<IList<SuggestedAlertsQueryResponse>> Handle(GetSuggestedsBasedOnLocationQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Latitude is null || request.Longitude is null)
        {
            return await _dbContext.AdoptionAlerts
                .AsNoTracking()
                .Include(alert => alert.Pet)
                .Include(alert => alert.Pet.Breed)
                .Include(alert => alert.Pet.Images)
                .Where(alert => alert.AdoptionDate == null)
                .OrderBy(alert => EF.Functions.Random())
                .Take(AmountOfSuggestedAlertsReturned)
                .Select(alert => new SuggestedAlertsQueryResponse(
                    alert.Id,
                    alert.AdoptionRestrictions,
                    alert.Location != null ? alert.Location.Y : null,
                    alert.Location != null ? alert.Location.X : null,
                    alert.Description,
                    alert.RegistrationDate,
                    alert.AdoptionDate,
                    new ExtraSimplifiedPetResponse(alert.Pet.Id,
                        alert.Pet.Name,
                        alert.Pet.Age.ToAgeResponse(),
                        alert.Pet.Breed.ToBreedResponse(),
                        alert.Pet.Gender.ToGenderResponse(),
                        alert.Pet.Images.Select(i => i.ImageUrl).ToList())
                ))
                .ToListAsync(cancellationToken);
        }

        const int defaultRecommendedRadiusInKm = 40;

        UserLocation location = new()
        {
            Latitude = request.Latitude.Value,
            Longitude = request.Longitude.Value
        };

        var filterLocation =
            CoordinatesCalculator.CreatePointBasedOnCoordinates(location.Latitude, location.Longitude);
        var filteredDistanceInMeters = UnitsConverter.ConvertKmToMeters(defaultRecommendedRadiusInKm);

        return await _dbContext.AdoptionAlerts
            .AsNoTracking()
            .Include(alert => alert.Pet)
            .Include(alert => alert.Pet.Breed)
            .Include(alert => alert.Pet.Images)
            .Include(alert => alert.User)
            .Where(alert =>
                alert.Location != null &&
                EF.Functions.IsWithinDistance(alert.Location, filterLocation, filteredDistanceInMeters, true) &&
                alert.AdoptionDate == null)
            .OrderBy(alert => EF.Functions.Random())
            .Take(AmountOfSuggestedAlertsReturned)
            .Select(alert => new SuggestedAlertsQueryResponse(
                alert.Id,
                alert.AdoptionRestrictions,
                alert.Location != null ? alert.Location.Y : null,
                alert.Location != null ? alert.Location.X : null,
                alert.Description,
                alert.RegistrationDate,
                alert.AdoptionDate,
                new ExtraSimplifiedPetResponse(alert.Pet.Id,
                    alert.Pet.Name,
                    alert.Pet.Age.ToAgeResponse(),
                    alert.Pet.Breed.ToBreedResponse(),
                    alert.Pet.Gender.ToGenderResponse(),
                    alert.Pet.Images.Select(i => i.ImageUrl).ToList())
            ))
            .ToListAsync(cancellationToken);
    }
}
using Application.Common.Interfaces.ExternalServices.GeoLocation;
using Application.Queries.GeoLocation.Common;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Queries.GeoLocation.GetCoordinatesFromStateAndCity;

public record GetCoordinatesFromStateAndCityQuery(
    string Neighborhood,
    string State,
    string City)
    : IRequest<GeoLocationResponse?>;

public class GetCoordinatesFromStateAndCityQueryHandler
    : IRequestHandler<GetCoordinatesFromStateAndCityQuery, GeoLocationResponse?>
{
    private readonly IGeoLocationClient _geoLocationClient;

    public GetCoordinatesFromStateAndCityQueryHandler(IGeoLocationClient geoLocationClient)
    {
        _geoLocationClient = Guard.Against.Null(geoLocationClient);
    }

    public async Task<GeoLocationResponse?> Handle(GetCoordinatesFromStateAndCityQuery request,
        CancellationToken cancellationToken)
    {
        return await _geoLocationClient.GetCoordinatesFromNeighborhoodStateAndCityAsync(
            request.Neighborhood,
            request.State,
            request.City);
    }
}
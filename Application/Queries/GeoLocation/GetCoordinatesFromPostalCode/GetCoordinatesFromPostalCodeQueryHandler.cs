using Application.Common.Interfaces.ExternalServices.GeoLocation;
using Application.Queries.GeoLocation.Common;
using Ardalis.GuardClauses;
using MediatR;

namespace Application.Queries.GeoLocation.GetCoordinatesFromPostalCode;

public record GetCoordinatesFromPostalCodeQuery(string PostalCode) : IRequest<GeoLocationResponse>;

public class GetCoordinatesFromPostalCodeQueryHandler
    : IRequestHandler<GetCoordinatesFromPostalCodeQuery, GeoLocationResponse>
{
    private readonly IGeoLocationClient _geoLocationClient;

    public GetCoordinatesFromPostalCodeQueryHandler(IGeoLocationClient geoLocationClient)
    {
        _geoLocationClient = Guard.Against.Null(geoLocationClient);
    }

    public async Task<GeoLocationResponse> Handle(GetCoordinatesFromPostalCodeQuery request,
        CancellationToken cancellationToken)
    {
        return await _geoLocationClient.GetCoordinatesViaPostalCodeAsync(request.PostalCode);
    }
}
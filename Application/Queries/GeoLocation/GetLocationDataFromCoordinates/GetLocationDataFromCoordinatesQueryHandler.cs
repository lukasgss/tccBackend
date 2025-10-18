using Application.Common.Interfaces.ExternalServices.GeoLocation;
using Application.Queries.GeoLocation.Common;
using Ardalis.GuardClauses;
using MediatR;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Queries.GeoLocation.GetLocationDataFromCoordinates;

public record GetLocationDataFromCoordinatesQuery(
    double? Latitude,
    double? Longitude)
    : IRequest<GeoLocationResponse>;

public class GetLocationDataFromCoordinatesQueryHandler
    : IRequestHandler<GetLocationDataFromCoordinatesQuery, GeoLocationResponse>
{
    private readonly IGeoLocationClient _geoLocationClient;

    public GetLocationDataFromCoordinatesQueryHandler(IGeoLocationClient geoLocationClient)
    {
        _geoLocationClient = Guard.Against.Null(geoLocationClient);
    }

    public async Task<GeoLocationResponse> Handle(GetLocationDataFromCoordinatesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Latitude is null || request.Longitude is null)
        {
            throw new NotFoundException("Campos de latitude e longitude são obrigatórios.");
        }

        GeoLocationResponse? locationData = await _geoLocationClient.GetLocationDataViaCoordinates(
            request.Latitude.Value,
            request.Longitude.Value);
        if (locationData is null)
        {
            throw new NotFoundException("Endereço com os dados especificados não foi encontrado.");
        }

        return locationData;
    }
}
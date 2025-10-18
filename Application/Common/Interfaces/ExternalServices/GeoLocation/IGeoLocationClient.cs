using Application.Queries.GeoLocation.Common;

namespace Application.Common.Interfaces.ExternalServices.GeoLocation;

public interface IGeoLocationClient
{
    Task<GeoLocationResponse> GetCoordinatesViaPostalCodeAsync(string postalCode);

    Task<GeoLocationResponse?> GetCoordinatesFromNeighborhoodStateAndCityAsync(
        string neighborhood, string state, string city);

    Task<GeoLocationResponse?> GetLocationDataViaCoordinates(double latitude, double longitude);
}
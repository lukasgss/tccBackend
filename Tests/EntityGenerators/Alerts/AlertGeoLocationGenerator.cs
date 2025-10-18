using System.Globalization;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Location;
using Application.Queries.GeoLocation.Common;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.EntityGenerators.Alerts;

public static class AlertGeoLocationGenerator
{
    public static AlertGeoLocation GenerateGeoLocation()
    {
        return new AlertGeoLocation()
        {
            Address = "Endereço",
            City = new LocationResponse(1, "São Paulo"),
            Neighborhood = "Neighborhood",
            State = new LocationResponse(1, "São Paulo"),
            PostalCode = "38762130"
        };
    }

    public static GeoLocationResponse GenerateGeoLocationResponse()
    {
        return new GeoLocationResponse
        {
            Address = "Endereço",
            City = "São Paulo",
            Neighborhood = "Neighborhood",
            State = "São Paulo",
            PostalCode = "38762130",
            Latitude = Constants.AdoptionAlertData.LocationLatitude.ToString(CultureInfo.InvariantCulture),
            Longitude = Constants.AdoptionAlertData.LocationLongitude.ToString(CultureInfo.InvariantCulture)
        };
    }
}
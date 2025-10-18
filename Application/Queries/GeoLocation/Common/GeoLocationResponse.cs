namespace Application.Queries.GeoLocation.Common;

public record GeoLocationResponse(
    string Latitude,
    string Longitude,
    string? Address,
    string? PostalCode,
    string? State,
    string? City,
    string? Neighborhood
);
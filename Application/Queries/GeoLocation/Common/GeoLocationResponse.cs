using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.GeoLocation.Common;

[ExcludeFromCodeCoverage]
public sealed record GeoLocationResponse(
    string Latitude,
    string Longitude,
    string? Address,
    string? PostalCode,
    string? State,
    string? City,
    string? Neighborhood
);
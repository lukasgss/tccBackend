using Application.Common.Interfaces.Entities.Location;

namespace Application.Common.GeoLocation;

public sealed record AlertGeoLocation(
    LocationResponse? City,
    string? Neighborhood,
    LocationResponse? State
);
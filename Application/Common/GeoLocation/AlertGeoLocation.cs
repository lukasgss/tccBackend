using Application.Common.Interfaces.Entities.Location;

namespace Application.Common.GeoLocation;

public record AlertGeoLocation(
    LocationResponse? City,
    string? Neighborhood,
    LocationResponse? State
);
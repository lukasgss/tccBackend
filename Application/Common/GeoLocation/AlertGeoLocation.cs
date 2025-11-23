using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Location;

namespace Application.Common.GeoLocation;

[ExcludeFromCodeCoverage]
public sealed record AlertGeoLocation(
    LocationResponse? City,
    string? Neighborhood,
    LocationResponse? State
);
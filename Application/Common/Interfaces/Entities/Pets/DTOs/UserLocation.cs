using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

[ExcludeFromCodeCoverage]
public sealed class UserLocation
{
	public required double Latitude { get; init; }
	public required double Longitude { get; init; }
}
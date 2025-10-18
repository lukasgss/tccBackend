using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Alerts;

public class BaseAlertFilters
{
	public double? Latitude { get; init; }
	public double? Longitude { get; init; }
	public double? RadiusDistanceInKm { get; init; }
	public List<int>? BreedIds { get; init; }
	public List<Gender>? GenderIds { get; init; }
	public int? SpeciesId { get; init; }
	public List<Age>? AgeIds { get; init; }
	public List<Size>? SizeIds { get; init; }
	public List<int>? ColorIds { get; init; }
}
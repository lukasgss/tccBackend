using Application.Common.Interfaces.Entities.Alerts;

namespace Application.Common.Extensions;

public static class BaseAlertFiltersExtensions
{
	public static bool HasGeoFilters(this BaseAlertFilters filters)
	{
		return filters.Latitude is not null && filters.Longitude is not null &&
		       filters.RadiusDistanceInKm is not null;
	}
}

using Application.Common.Interfaces.Entities.Alerts;

namespace Application.Common.Extensions;

public static class BaseAlertFiltersExtensions
{
	extension(BaseAlertFilters filters)
	{
		public bool HasGeoFilters()
		{
			return filters.Latitude is not null && filters.Longitude is not null &&
			       filters.RadiusDistanceInKm is not null;
		}
	}
}
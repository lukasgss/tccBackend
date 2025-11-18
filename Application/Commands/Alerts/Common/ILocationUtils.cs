using Application.Common.Interfaces.General.Location;
using NetTopologySuite.Geometries;

namespace Application.Commands.Alerts.Common;

public interface ILocationUtils
{
	Task<Point> GetAlertLocation(AlertLocalization localizationData, string neighborhood);

	Task<AlertLocalization> GetAlertStateAndCity(int stateId, int cityId);
}
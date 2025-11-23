using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Converters;

[ExcludeFromCodeCoverage]
public static class UnitsConverter
{
	public static double ConvertKmToMeters(double measurementInKm)
	{
		return measurementInKm * 1000;
	}
}
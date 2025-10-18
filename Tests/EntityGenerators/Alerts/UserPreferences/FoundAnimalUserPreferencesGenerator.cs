using Application.Common.Calculators;
using Domain.Entities.Alerts.UserPreferences;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.UserPreferences;

public static class FoundAnimalUserPreferencesGenerator
{
	public static FoundAnimalUserPreferences GenerateFoundAnimalUserPreferences()
	{
		return new FoundAnimalUserPreferences()
		{
			Id = Constants.FoundAnimalUserPreferencesData.Id,
			User = Constants.FoundAnimalUserPreferencesData.User,
			UserId = Constants.FoundAnimalUserPreferencesData.User.Id,
			Colors = Constants.FoundAnimalUserPreferencesData.Colors,
			Ages = Constants.FoundAnimalUserPreferencesData.Ages,
			Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
				Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude!.Value,
				Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude!.Value),
			RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
			Species = Constants.FoundAnimalUserPreferencesData.Species,
			Breeds = Constants.FoundAnimalUserPreferencesData.Breeds,
			Genders = Constants.FoundAnimalUserPreferencesData.Genders
		};
	}
}
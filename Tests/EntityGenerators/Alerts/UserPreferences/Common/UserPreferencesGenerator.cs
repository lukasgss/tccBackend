using Application.Common.DTOs;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts.UserPreferences.Common;

public static class UserPreferencesGenerator
{
    public static UserPreferencesRequest GenerateCreateFoundAnimalUserPreferences()
    {
        return new UserPreferencesRequest()
        {
            BreedIds = Constants.FoundAnimalUserPreferencesData.BreedIds,
            SpeciesIds = Constants.FoundAnimalUserPreferencesData.SpeciesIds,
            ColorIds = Constants.FoundAnimalUserPreferencesData.ColorIds,
            FoundLocationLatitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLatitude,
            FoundLocationLongitude = Constants.FoundAnimalUserPreferencesData.FoundLocationLongitude,
            Sizes = Constants.FoundAnimalUserPreferencesData.Sizes,
            Ages = Constants.FoundAnimalUserPreferencesData.Ages,
            RadiusDistanceInKm = Constants.FoundAnimalUserPreferencesData.RadiusDistanceInKm,
            Genders = Constants.FoundAnimalUserPreferencesData.Genders
        };
    }
}
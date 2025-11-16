using Application.Common.DTOs;
using Domain.Entities.Alerts.UserPreferences;

namespace Application.Common.Extensions.Mapping.Alerts.UserPreferences;

public static class UserPreferencesMappings
{
    extension(FoundAnimalUserPreferences userPreferences)
    {
        public UserPreferencesResponse ToFoundAnimalUserPreferencesResponse()
        {
            return new UserPreferencesResponse(
                Id: userPreferences.Id,
                User: userPreferences.User.ToUserDataResponse(),
                Colors: userPreferences.Colors.ToListOfColorResponse(),
                Species: userPreferences.Species.ToListOfSpeciesResponse(),
                Ages: userPreferences.Ages.ToListOfAgeResponse(),
                Breeds: userPreferences.Breeds.ToListOfBreedResponse(),
                Genders: userPreferences.Genders.ToListOfGenderResponse(),
                Sizes: userPreferences.Sizes.ToListOfSizeResponse(),
                FoundLocationLatitude: userPreferences.Location?.Y,
                FoundLocationLongitude: userPreferences.Location?.X,
                RadiusDistanceInKm: userPreferences.RadiusDistanceInKm
            );
        }
    }

    extension(AdoptionUserPreferences userPreferences)
    {
        public UserPreferencesResponse ToAdoptionUserPreferencesResponse()
        {
            return new UserPreferencesResponse(
                Id: userPreferences.Id,
                User: userPreferences.User.ToUserDataResponse(),
                Ages: userPreferences.Ages.ToListOfAgeResponse(),
                Species: userPreferences.Species.ToListOfSpeciesResponse(),
                Breeds: userPreferences.Breeds.ToListOfBreedResponse(),
                Genders: userPreferences.Genders.ToListOfGenderResponse(),
                Sizes: userPreferences.Sizes.ToListOfSizeResponse(),
                Colors: userPreferences.Colors.ToListOfColorResponse(),
                FoundLocationLatitude: userPreferences.Location?.Y,
                FoundLocationLongitude: userPreferences.Location?.X,
                RadiusDistanceInKm: userPreferences.RadiusDistanceInKm
            );
        }
    }
}
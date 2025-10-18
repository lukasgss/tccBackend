using Application.Queries.Colors.GetAll;
using Application.Queries.Users.Common;

namespace Application.Common.DTOs;

public record UserPreferencesResponse(
    Guid Id,
    double? FoundLocationLatitude,
    double? FoundLocationLongitude,
    double? RadiusDistanceInKm,
    List<GenderResponse> Genders,
    List<AgeResponse> Ages,
    List<SizeResponse> Sizes,
    List<SpeciesResponse> Species,
    List<BreedResponse> Breeds,
    List<ColorResponse> Colors,
    UserDataResponse User
);
using Application.Common.DTOs;
using Application.Queries.Colors.GetAll;
using Application.Queries.Users.Common;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public record FoundAnimalAlertResponse(
    Guid Id,
    string? Name,
    string? Description,
    double FoundLocationLatitude,
    double FoundLocationLongitude,
    DateTime RegistrationDate,
    DateOnly? RecoveryDate,
    List<string> Images,
    string Age,
    SpeciesResponse Species,
    BreedResponse? Breed,
    UserDataResponse Owner,
    string? Gender,
    string Size,
    IEnumerable<ColorResponse> Colors
);
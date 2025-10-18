using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public record AdoptionAlertProfileListing(
    Guid Id,
    List<string> AdoptionRestrictions,
    double? LocationLatitude,
    double? LocationLongitude,
    string? Description,
    bool IsFavorite,
    DateTime RegistrationDate,
    DateOnly? AdoptionDate,
    ExtraSimplifiedPetResponse Pet);
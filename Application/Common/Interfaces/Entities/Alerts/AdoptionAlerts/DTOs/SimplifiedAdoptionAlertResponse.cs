using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public record SimplifiedAdoptionAlertResponse(
    Guid Id,
    List<string> AdoptionRestrictions,
    double? LocationLatitude,
    double? LocationLongitude,
    string? Description,
    DateTime RegistrationDate,
    DateOnly? AdoptionDate,
    SimplifiedPetResponse Pet
);
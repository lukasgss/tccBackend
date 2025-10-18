using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Common.DTOs;

public record MissingAlertResponse(
    Guid Id,
    DateTime RegistrationDate,
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    DateOnly? RecoveryDate,
    PetResponseNoOwner Pet,
    OwnerResponse Owner
);
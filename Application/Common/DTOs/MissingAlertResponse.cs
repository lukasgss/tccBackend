using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;

namespace Application.Common.DTOs;

public sealed record MissingAlertResponse(
    Guid Id,
    DateTime RegistrationDate,
    City City,
    State State,
    string Neighborhood,
    string? Description,
    DateOnly? RecoveryDate,
    PetResponseNoOwner Pet,
    OwnerResponse Owner
);
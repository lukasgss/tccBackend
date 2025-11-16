using Application.Common.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using NetTopologySuite.Geometries;

namespace Application.Queries.MissingAlerts.ListMissingAlerts;

public record MissingAlertQueryResponse(
    Guid Id,
    DateTime RegistrationDate,
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    DateOnly? RecoveryDate,
    PetResponseNoOwner Pet,
    OwnerResponse Owner,
    Point Location
) : MissingAlertResponse(Id,
    RegistrationDate,
    LastSeenLocationLatitude,
    LastSeenLocationLongitude,
    Description,
    RecoveryDate,
    Pet,
    Owner);
using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;

namespace Application.Queries.MissingAlerts.ListMissingAlerts;

[ExcludeFromCodeCoverage]
public record MissingAlertQueryResponse(
    Guid Id,
    DateTime RegistrationDate,
    State State,
    City City,
    string Neighborhood,
    string? Description,
    DateOnly? RecoveryDate,
    PetResponseNoOwner Pet,
    OwnerResponse Owner);
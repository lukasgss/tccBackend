using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;
using Domain.Common;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record AdoptionAlertResponse(
    Guid Id,
    List<string> AdoptionRestrictions,
    double? LocationLatitude,
    double? LocationLongitude,
    string Neighborhood,
    string? Description,
    FileAttachment? AdoptionForm,
    DateTime RegistrationDate,
    DateOnly? AdoptionDate,
    PetResponseNoOwner Pet,
    UserDataResponse Owner
);
using System.Diagnostics.CodeAnalysis;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;
using Domain.Common;

namespace Application.Queries.AdoptionFavorites.GetById;

[ExcludeFromCodeCoverage]
public record AdoptionAlertResponseWithGeoLocation(
    Guid Id,
    List<string> AdoptionRestrictions,
    double? LocationLatitude,
    double? LocationLongitude,
    string Neighborhood,
    int StateId,
    int CityId,
    string? Description,
    DateTime RegistrationDate,
    DateOnly? AdoptionDate,
    bool IsFavorite,
    PetResponseNoOwner Pet,
    AlertUserDataResponse Owner,
    AlertGeoLocation? GeoLocation,
    FileAttachment? AdoptionForm
);
using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Queries.Colors.GetAll;

namespace Application.Common.Interfaces.SharingAlerts;

[ExcludeFromCodeCoverage]
public sealed record AdoptionAlertPosterData(
    string PetName,
    string Image,
    SpeciesResponse Species,
    BreedResponse Breed,
    GenderResponse Sex,
    bool? IsCastrated,
    bool? IsNegativeToFivFelv,
    AgeResponse Age,
    SizeResponse Size,
    List<ColorResponse> Colors,
    bool? IsVaccinated,
    string? Description,
    List<string> AdoptionRestrictions,
    string ContactName,
    string ContactPhoneNumber);
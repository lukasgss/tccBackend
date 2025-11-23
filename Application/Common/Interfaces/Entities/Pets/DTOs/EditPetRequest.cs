using System.Diagnostics.CodeAnalysis;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

[ExcludeFromCodeCoverage]
public sealed record EditPetRequest(
    string Name,
    Gender Gender,
    Age Age,
    List<IFormFile>? Images,
    List<string> ExistingImages,
    bool? IsCastrated,
    bool? IsVaccinated,
    bool? IsNegativeToFivFelv,
    bool? IsNegativeToLeishmaniasis,
    int BreedId,
    int SpeciesId,
    Size Size,
    List<int> ColorIds
);
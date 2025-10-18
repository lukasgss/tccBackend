using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public record CreatePetRequest(
    string Name,
    Gender Gender,
    Size Size,
    Age Age,
    bool? IsCastrated,
    bool? IsVaccinated,
    bool? IsNegativeToFivFelv,
    bool? IsNegativeToLeishmaniasis,
    List<IFormFile> Images,
    int BreedId,
    int SpeciesId,
    List<int> ColorIds);
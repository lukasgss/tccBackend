using Application.Common.DTOs;
using Application.Queries.Colors.GetAll;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public record PetResponse(
    Guid Id,
    string Name,
    GenderResponse Gender,
    AgeResponse Age,
    List<string> Images,
    OwnerResponse? Owner,
    SizeResponse Size,
    IEnumerable<ColorResponse> Colors,
    BreedResponse Breed,
    SpeciesResponse Species
);
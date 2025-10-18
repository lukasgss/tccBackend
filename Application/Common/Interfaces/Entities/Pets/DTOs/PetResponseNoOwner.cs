using Application.Common.DTOs;
using Application.Queries.Colors.GetAll;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public record PetResponseNoOwner(
    Guid Id,
    string Name,
    GenderResponse Gender,
    AgeResponse Age,
    SizeResponse Size,
    List<string> Images,
    IEnumerable<ColorResponse> Colors,
    bool? IsVaccinated,
    bool? IsCastrated,
    bool? IsNegativeToFivFelv,
    bool? IsNegativeToLeishmaniasis,
    BreedResponse Breed,
    SpeciesResponse Species
);
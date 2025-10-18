using Application.Common.DTOs;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public record ExtraSimplifiedPetResponse(
    Guid Id,
    string Name,
    AgeResponse Age,
    BreedResponse Breed,
    GenderResponse Gender,
    List<string> Images
);
using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

[ExcludeFromCodeCoverage]
public sealed record ExtraSimplifiedPetResponse(
    Guid Id,
    string Name,
    AgeResponse Age,
    BreedResponse Breed,
    GenderResponse Gender,
    List<string> Images
);
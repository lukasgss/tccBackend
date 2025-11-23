using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

[ExcludeFromCodeCoverage]
public sealed record SimplifiedPetResponse(
    Guid Id,
    string Name,
    string Gender,
    string Age,
    string Size,
    List<string> Images
);
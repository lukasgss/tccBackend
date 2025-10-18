namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public record SimplifiedPetResponse(
    Guid Id,
    string Name,
    string Gender,
    string Age,
    string Size,
    List<string> Images
);
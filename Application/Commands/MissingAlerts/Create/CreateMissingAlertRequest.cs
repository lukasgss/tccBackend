using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Commands.MissingAlerts.Create;

public record CreateMissingAlertRequest(
    int City,
    int State,
    string Neighborhood,
    string? Description,
    CreatePetRequest Pet
);
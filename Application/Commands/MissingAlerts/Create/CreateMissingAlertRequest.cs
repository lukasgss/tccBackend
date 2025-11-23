using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Commands.MissingAlerts.Create;

[ExcludeFromCodeCoverage]
public sealed record CreateMissingAlertRequest(
    int City,
    int State,
    string Neighborhood,
    string? Description,
    CreatePetRequest Pet
);
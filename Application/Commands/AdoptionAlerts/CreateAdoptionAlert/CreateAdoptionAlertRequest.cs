using Application.Common.Interfaces.Entities.Pets.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.AdoptionAlerts.CreateAdoptionAlert;

public record CreateAdoptionAlertRequest(
    string Neighborhood,
    int State,
    int City,
    string? Description,
    CreatePetRequest Pet,
    IFormFile? AdoptionForm,
    bool ShouldUseDefaultAdoptionForm,
    List<string>? AdoptionRestrictions,
    bool ForceCreationWithNotFoundCoordinates
)
{
    public List<string> AdoptionRestrictions { get; init; } = AdoptionRestrictions ?? [];
}
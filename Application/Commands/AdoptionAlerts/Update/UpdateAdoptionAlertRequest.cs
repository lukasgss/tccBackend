using Application.Common.Interfaces.Entities.Pets.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.AdoptionAlerts.Update;

public record UpdateAdoptionAlertRequest(
    List<string>? AdoptionRestrictions,
    string Neighborhood,
    int State,
    int City,
    string? Description,
    EditPetRequest Pet,
    IFormFile? AdoptionForm,
    string? ExistingAdoptionFormUrl,
    bool ShouldUseDefaultAdoptionForm,
    bool ForceCreationWithNotFoundCoordinates)
{
    public List<string> AdoptionRestrictions { get; init; } = AdoptionRestrictions ?? [];
}
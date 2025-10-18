using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Files;

public interface IPetImageSubmissionService
{
    Task<List<string>> UploadPetImageAsync(List<IFormFile> petImages);
    Task<List<string>> UpdatePetImageAsync(Guid petId, List<IFormFile> newlyAddedImages);
    Task DeletePetImageAsync(Guid petId, List<PetImage> petImages);
}
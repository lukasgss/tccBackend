using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.General.Files;

public interface IAdoptionAlertFileSubmissionService
{
    Task<string?> UploadAdoptionFormAsync(
        IFormFile? file,
        string? previousAdoptionFormUrl,
        string? userDefaultAdoptionFormUrl,
        bool shouldUseDefaultAdoptionForm);

    Task<string> UploadUserDefaultAdoptionFormAsync(IFormFile file, string? previousUserDefaultAdoptionFormUrl);
}
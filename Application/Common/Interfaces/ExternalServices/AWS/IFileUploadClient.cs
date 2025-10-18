using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.ExternalServices.AWS;

public interface IFileUploadClient
{
    Task<AwsS3FileResponse> UploadPetImageAsync(MemoryStream imageStream, IFormFile imageFile, Guid imageId);
    Task<AwsS3FileResponse> UploadUserImageAsync(MemoryStream? imageStream, IFormFile? imageFile, Guid imageId);
    Task<AwsS3FileResponse> UploadAdoptionFormFileAsync(MemoryStream? fileStream, IFormFile formFile, Guid fileId);

    Task<AwsS3FileResponse> UploadFoundAlertImageAsync(
        MemoryStream imageStream, IFormFile imageFile, string hashedAlertId);

    Task<AwsS3FileResponse> DeletePetImageAsync(PetImage petImage);
    Task<AwsS3FileResponse> DeleteFoundAlertImageAsync(string hashedAlertId);
    Task<AwsS3FileResponse> DeletePreviousUserImageAsync(string userImageUrl);
    Task<AwsS3FileResponse> DeletePreviousAdoptionForm(string fileUrl);
    string FormatPublicUrlString(string? imageKey);
}
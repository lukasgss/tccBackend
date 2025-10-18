using Application.Common.Exceptions;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services.General.Files;

public class AdoptionAlertFileSubmissionService : IAdoptionAlertFileSubmissionService
{
    private readonly IFileUploadClient _fileUploadClient;
    private readonly ILogger<AdoptionAlertFileSubmissionService> _logger;

    public AdoptionAlertFileSubmissionService(
        IFileUploadClient fileUploadClient,
        ILogger<AdoptionAlertFileSubmissionService> logger)
    {
        _fileUploadClient = Guard.Against.Null(fileUploadClient);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<string?> UploadAdoptionFormAsync(
        IFormFile? file,
        string? previousAdoptionFormUrl,
        string? userDefaultAdoptionFormUrl,
        bool shouldUseDefaultAdoptionForm)
    {
        if (!string.IsNullOrEmpty(userDefaultAdoptionFormUrl) && shouldUseDefaultAdoptionForm)
        {
            await DeletePreviousAdoptionForm(previousAdoptionFormUrl);
            return userDefaultAdoptionFormUrl;
        }

        if (file is null)
        {
            await DeletePreviousAdoptionForm(previousAdoptionFormUrl);
            return null;
        }

        await using MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);

        Guid fileId = Guid.NewGuid();

        AwsS3FileResponse uploadedFile = await _fileUploadClient.UploadAdoptionFormFileAsync(
            fileStream: memoryStream,
            formFile: file,
            fileId);

        if (!uploadedFile.Success || uploadedFile.PublicUrl is null)
        {
            _logger.LogInformation("Não foi possível inserir o arquivo {FileId}", fileId);
            throw new InternalServerErrorException(
                "Não foi possível fazer upload do arquivo, tente novamente mais tarde.");
        }

        await DeletePreviousAdoptionForm(previousAdoptionFormUrl);

        return uploadedFile.PublicUrl;
    }

    public async Task<string> UploadUserDefaultAdoptionFormAsync(IFormFile file,
        string? previousUserDefaultAdoptionFormUrl)
    {
        await using MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);

        Guid fileId = Guid.NewGuid();

        AwsS3FileResponse uploadedFile = await _fileUploadClient.UploadAdoptionFormFileAsync(
            fileStream: memoryStream,
            formFile: file,
            fileId);

        if (!uploadedFile.Success || uploadedFile.PublicUrl is null)
        {
            _logger.LogInformation("Não foi possível inserir o arquivo {FileId}", fileId);
            throw new InternalServerErrorException(
                "Não foi possível fazer upload do arquivo, tente novamente mais tarde.");
        }

        await DeletePreviousAdoptionForm(previousUserDefaultAdoptionFormUrl);

        return uploadedFile.PublicUrl;
    }

    private async Task DeletePreviousAdoptionForm(string? previousUserDefaultAdoptionFormUrl)
    {
        if (previousUserDefaultAdoptionFormUrl is not null)
        {
            var deleteResult = await _fileUploadClient.DeletePreviousAdoptionForm(previousUserDefaultAdoptionFormUrl);
            if (!deleteResult.Success)
            {
                _logger.LogError(
                    "Error to delete previous user default adoption form {PreviousUserDefaultAdoptionFormUrl}",
                    previousUserDefaultAdoptionFormUrl);
            }
        }
    }
}
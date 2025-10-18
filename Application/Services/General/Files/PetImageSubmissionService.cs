using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services.General.Files;

public class PetImageSubmissionService : IPetImageSubmissionService
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IFileUploadClient _fileUploadClient;
    private readonly IPetImageRepository _petImageRepository;
    private readonly ILogger<PetImageSubmissionService> _logger;

    public PetImageSubmissionService(
        IImageProcessingService imageProcessingService,
        IFileUploadClient fileUploadClient,
        IPetImageRepository petImageRepository,
        ILogger<PetImageSubmissionService> logger)
    {
        _imageProcessingService =
            imageProcessingService ?? throw new ArgumentNullException(nameof(imageProcessingService));
        _fileUploadClient = fileUploadClient ?? throw new ArgumentNullException(nameof(fileUploadClient));
        _petImageRepository = petImageRepository ?? throw new ArgumentNullException(nameof(petImageRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<string>> UploadPetImageAsync(List<IFormFile> petImages)
    {
        return await UploadImages(petImages);
    }

    public async Task<List<string>> UpdatePetImageAsync(Guid petId, List<IFormFile> newlyAddedImages)
    {
        List<PetImage> previousImages = await _petImageRepository.GetImagesFromPetByIdAsync(petId);
        await DeletePreviousPetImagesAsync(previousImages);

        List<string> uploadedImages = await UploadImages(newlyAddedImages);

        return uploadedImages;
    }

    public async Task DeletePetImageAsync(Guid petId, List<PetImage> petImages)
    {
        foreach (PetImage petImage in petImages)
        {
            AwsS3FileResponse response = await _fileUploadClient.DeletePetImageAsync(petImage);
            if (!response.Success)
            {
                _logger.LogInformation("Não foi possível excluir a imagem {ImageUrl}", petImage.ImageUrl);
                throw new InternalServerErrorException(
                    "Não foi possível excluir a imagem do animal, tente novamente mais tarde.");
            }
        }
    }

    private async Task<List<string>> UploadImages(List<IFormFile> images)
    {
        List<string> uploadedImages = new(images.Count);

        foreach (var image in images)
        {
            await using MemoryStream compressedImage =
                await _imageProcessingService.CompressImageAsync(image.OpenReadStream());

            Guid imageId = Guid.NewGuid();

            AwsS3FileResponse uploadedFile = await _fileUploadClient.UploadPetImageAsync(
                imageStream: compressedImage,
                imageFile: image,
                imageId);

            if (!uploadedFile.Success || uploadedFile.PublicUrl is null)
            {
                _logger.LogInformation("Não foi possível inserir a imagem {ImageId}", imageId);
                throw new InternalServerErrorException(
                    "Não foi possível fazer upload da imagem, tente novamente mais tarde.");
            }

            uploadedImages.Add(uploadedFile.PublicUrl);
        }

        return uploadedImages;
    }

    private async Task DeletePreviousPetImagesAsync(List<PetImage> images)
    {
        foreach (PetImage petImage in images)
        {
            AwsS3FileResponse response = await _fileUploadClient.DeletePetImageAsync(petImage);
            if (!response.Success)
            {
                _logger.LogInformation("Não foi possível excluir a imagem {ImageUrl}", petImage.ImageUrl);
                throw new InternalServerErrorException(
                    "Não foi possível excluir a imagem, tente novamente mais tarde.");
            }
        }
    }
}
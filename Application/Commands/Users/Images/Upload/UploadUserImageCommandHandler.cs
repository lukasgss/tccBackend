using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Commands.Users.Images.Upload;

[ExcludeFromCodeCoverage]
public sealed record UploadUserImageCommand(IFormFile? UserImage, string? PreviousImageUrl) : IRequest<string>;

public sealed class UploadUserImageCommandHandler : IRequestHandler<UploadUserImageCommand, string>
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IFileUploadClient _fileUploadClient;
    private readonly ImagesData _imagesData;
    private readonly ILogger<UploadUserImageCommandHandler> _logger;

    public UploadUserImageCommandHandler(
        IImageProcessingService imageProcessingService,
        IFileUploadClient fileUploadClient,
        IOptions<ImagesData> imagesData,
        ILogger<UploadUserImageCommandHandler> logger)
    {
        _imageProcessingService = Guard.Against.Null(imageProcessingService);
        _fileUploadClient = Guard.Against.Null(fileUploadClient);
        _imagesData = Guard.Against.Null(imagesData.Value);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<string> Handle(UploadUserImageCommand request, CancellationToken cancellationToken)
    {
        await using MemoryStream? compressedImage = request.UserImage is not null
            ? await _imageProcessingService.CompressImageAsync(request.UserImage.OpenReadStream())
            : null;

        Guid imageId = Guid.NewGuid();

        AwsS3FileResponse uploadedFile = await _fileUploadClient.UploadUserImageAsync(
            imageStream: compressedImage,
            imageFile: request.UserImage,
            imageId);

        if (!uploadedFile.Success || uploadedFile.PublicUrl is null)
        {
            _logger.LogInformation("Não foi possível inserir a imagem {ImageId}", imageId);
            throw new InternalServerErrorException(
                "Não foi possível fazer upload da imagem, tente novamente mais tarde.");
        }

        if (request.PreviousImageUrl is not null &&
            !request.PreviousImageUrl.Contains(_imagesData.DefaultUserProfilePicture))
        {
            await _fileUploadClient.DeletePreviousUserImageAsync(request.PreviousImageUrl);
        }

        return uploadedFile.PublicUrl;
    }
}
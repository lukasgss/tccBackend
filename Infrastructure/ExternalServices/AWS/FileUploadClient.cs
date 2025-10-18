using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Domain.ValueObjects;
using Infrastructure.ExternalServices.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.AWS;

public class FileUploadClient : IFileUploadClient
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsData _awsData;
    private readonly ImagesData _imagesData;
    private readonly ILogger<FileUploadClient> _logger;

    public FileUploadClient(
        IAmazonS3 s3Client,
        IOptions<AwsData> awsData,
        IOptions<ImagesData> imagesData,
        ILogger<FileUploadClient> logger)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        _awsData = awsData.Value ?? throw new ArgumentNullException(nameof(awsData));
        _imagesData = imagesData.Value ?? throw new ArgumentNullException(nameof(imagesData));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AwsS3FileResponse> UploadPetImageAsync(
        MemoryStream imageStream, IFormFile imageFile, Guid imageId)
    {
        try
        {
            PutObjectRequest putObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                // Content type and image extension is always set to webp, since
                // the image service always encodes the image as webp format
                Key = $"Images/{_awsData.PetImagesFolder}/{imageId}.webp",
                ContentType = "image/webp",
                InputStream = imageStream,
                Metadata =
                {
                    ["x-amz-meta-originalname"] = imageFile.FileName,
                    ["x-amz-meta-extension"] = ".webp"
                },
                DisablePayloadSigning = true
            };
            await _s3Client.PutObjectAsync(putObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = FormatPublicUrlString(putObjectRequest.Key)
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> UploadUserImageAsync(
        MemoryStream? imageStream, IFormFile? imageFile, Guid imageId)
    {
        try
        {
            if (imageFile is null)
            {
                return new AwsS3FileResponse()
                {
                    Success = true,
                    PublicUrl = FormatPublicUrlString(_imagesData.DefaultUserProfilePicture)
                };
            }

            PutObjectRequest putObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                // Content type and image extension is always set to webp, since
                // the image service always encodes the image as webp format
                Key = $"Images/{_awsData.UserImagesFolder}/{imageId}.webp",
                ContentType = "image/webp",
                InputStream = imageStream,
                Metadata =
                {
                    ["x-amz-meta-originalname"] = imageFile.FileName,
                    ["x-amz-meta-extension"] = ".webp"
                },
                DisablePayloadSigning = true
            };
            await _s3Client.PutObjectAsync(putObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = FormatPublicUrlString(putObjectRequest.Key)
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> UploadAdoptionFormFileAsync(
        MemoryStream? fileStream, IFormFile formFile, Guid fileId)
    {
        try
        {
            string fileExtension = Path.GetExtension(formFile.FileName);

            PutObjectRequest putObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                Key = $"Files/{_awsData.AdoptionFormsFolder}/{fileId}{fileExtension}",
                ContentType = formFile.ContentType,
                InputStream = fileStream,
                Metadata =
                {
                    ["x-amz-meta-originalname"] = formFile.FileName,
                    ["x-amz-meta-extension"] = fileExtension
                },
                DisablePayloadSigning = true
            };
            await _s3Client.PutObjectAsync(putObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = FormatPublicUrlString(putObjectRequest.Key)
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> UploadFoundAlertImageAsync(
        MemoryStream imageStream, IFormFile imageFile, string hashedId)
    {
        try
        {
            PutObjectRequest putObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                // Content type and image extension is always set to webp, since
                // the image service always encodes the image as webp format
                Key = $"Images/{_awsData.FoundAlertImagesFolder}/{hashedId}.webp",
                ContentType = "image/webp",
                InputStream = imageStream,
                Metadata =
                {
                    ["x-amz-meta-originalname"] = imageFile.FileName,
                    ["x-amz-meta-extension"] = ".webp"
                },
                DisablePayloadSigning = true
            };
            await _s3Client.PutObjectAsync(putObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = FormatPublicUrlString(putObjectRequest.Key)
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> DeletePetImageAsync(PetImage petImage)
    {
        try
        {
            string imageId = ExtractPetImageId(petImage.ImageUrl);
            DeleteObjectRequest deleteObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                Key = $"Images/{_awsData.PetImagesFolder}/{imageId}.webp"
            };
            await _s3Client.DeleteObjectAsync(deleteObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = null
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> DeleteFoundAlertImageAsync(string hashedAlertId)
    {
        try
        {
            DeleteObjectRequest deleteObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                Key = $"Images/{_awsData.FoundAlertImagesFolder}/{hashedAlertId}.webp"
            };
            await _s3Client.DeleteObjectAsync(deleteObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = null
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> DeletePreviousUserImageAsync(string userImageUrl)
    {
        try
        {
            DeleteObjectRequest deleteObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                Key = $"Images/{_awsData.UserImagesFolder}/{ExtractUserImageId(userImageUrl)}.webp"
            };
            await _s3Client.DeleteObjectAsync(deleteObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = null
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    public async Task<AwsS3FileResponse> DeletePreviousAdoptionForm(string fileUrl)
    {
        try
        {
            DeleteObjectRequest deleteObjectRequest = new()
            {
                BucketName = _awsData.BucketName,
                Key = $"Files/{_awsData.AdoptionFormsFolder}/{ExtractAdoptionFormFileId(fileUrl)}"
            };
            await _s3Client.DeleteObjectAsync(deleteObjectRequest);

            return new AwsS3FileResponse()
            {
                Success = true,
                PublicUrl = null
            };
        }
        catch (AmazonS3Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            return new AwsS3FileResponse()
            {
                Success = false,
                PublicUrl = null
            };
        }
    }

    private static string ExtractPetImageId(string url)
    {
        const string pattern = @"/Pets/([^/.]+)\.webp";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(url);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        throw new ArgumentException("URL com formato inválida.");
    }

    private static string ExtractUserImageId(string url)
    {
        const string pattern = @"/User/([^/.]+)\.webp";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(url);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        throw new ArgumentException("URL com formato inválida.");
    }

    private string ExtractAdoptionFormFileId(string url)
    {
        string pattern = @$"/{_awsData.AdoptionFormsFolder}/([^/.]+\.[a-zA-Z0-9]+)";

        Regex regex = new Regex(pattern);
        Match match = regex.Match(url);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        throw new ArgumentException("Invalid URL format.");
    }

    public string FormatPublicUrlString(string? imageKey)
    {
        return imageKey is null
            ? $"{_awsData.ServiceDomain}/{_imagesData.DefaultUserProfilePicture}"
            : $"{_awsData.ServiceDomain}/{imageKey}";
    }
}
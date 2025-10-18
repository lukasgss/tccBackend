using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Application.Services.General.Files;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tests.EntityGenerators;

namespace Tests.UnitTests.Images;

public class PetImageSubmissionServiceTests
{
    private readonly IFileUploadClient _fileUploadClientMock;
    private readonly IIdConverterService _idConverterServiceMock;
    private readonly IPetImageSubmissionService _sut;

    private static readonly Pet Pet = PetGenerator.GeneratePet();
    private static readonly CreatePetRequest CreatePetRequest = PetGenerator.GenerateCreatePetRequest();

    private static readonly AwsS3FileResponse S3FailFileResponse =
        AwsS3ImageGenerator.GenerateFailS3ImageResponse();

    private static readonly AwsS3FileResponse S3SuccessFileResponse =
        AwsS3ImageGenerator.GenerateSuccessS3ImageResponse();

    public PetImageSubmissionServiceTests()
    {
        IImageProcessingService imageProcessingServiceMock = Substitute.For<IImageProcessingService>();
        _fileUploadClientMock = Substitute.For<IFileUploadClient>();
        _idConverterServiceMock = Substitute.For<IIdConverterService>();
        var petImageRepositoryMock = Substitute.For<IPetImageRepository>();
        var loggerMock = Substitute.For<ILogger<PetImageSubmissionService>>();

        _sut = new PetImageSubmissionService(imageProcessingServiceMock,
            _fileUploadClientMock,
            petImageRepositoryMock,
            loggerMock);
    }

    [Fact]
    public async Task Failed_Pet_Image_Upload_Throws_InternalServerErrorException()
    {
        _fileUploadClientMock
            .UploadPetImageAsync(Arg.Any<MemoryStream>(), CreatePetRequest.Images.First(), Arg.Any<Guid>())
            .Returns(S3FailFileResponse);

        async Task Result() => await _sut.UploadPetImageAsync(CreatePetRequest.Images);

        var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
        Assert.Equal("Não foi possível fazer upload da imagem, tente novamente mais tarde.", exception.Message);
    }

    [Fact]
    public async Task Failed_Delete_Pet_Image_Throws_InternalServerErrorException()
    {
        const string hashedId = "hashedId";
        _idConverterServiceMock.ConvertGuidToShortId(Pet.Id, Arg.Any<int>()).Returns(hashedId);
        _fileUploadClientMock.DeletePetImageAsync(Arg.Any<PetImage>()).Returns(S3FailFileResponse);
        List<PetImage> petImages = new()
        {
            new PetImage() { Id = 1, Pet = PetGenerator.GeneratePet(), ImageUrl = "imageUrl" }
        };

        async Task Result() => await _sut.DeletePetImageAsync(Pet.Id, petImages);

        var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
        Assert.Equal("Não foi possível excluir a imagem do animal, tente novamente mais tarde.", exception.Message);
    }

    [Fact]
    public async Task Pet_Image_Upload_Returns_Uploaded_Image_Url()
    {
        _fileUploadClientMock
            .UploadPetImageAsync(Arg.Any<MemoryStream>(), CreatePetRequest.Images.First(),
                Arg.Any<Guid>())
            .Returns(S3SuccessFileResponse);
        List<string> expectedUrls = new() { S3SuccessFileResponse.PublicUrl! };

        var uploadedUrls = await _sut.UploadPetImageAsync(CreatePetRequest.Images);

        Assert.Equal(expectedUrls, uploadedUrls);
    }
}
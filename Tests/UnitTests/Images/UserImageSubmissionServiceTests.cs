using System.IO;
using Application.Commands.Users.Edit;
using Application.Common.Exceptions;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tests.EntityGenerators;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests.Images;

public class UserImageSubmissionServiceTests
{
    private readonly IFileUploadClient _fileUploadClientMock;
    private readonly IUserImageSubmissionService _sut;

    private static readonly User User = UserGenerator.GenerateUser();
    private static readonly EditUserRequest EditUserRequest = UserGenerator.GenerateEditUserRequest();

    private static readonly AwsS3FileResponse S3FailFileResponse =
        AwsS3ImageGenerator.GenerateFailS3ImageResponse();

    private static readonly AwsS3FileResponse S3SuccessFileResponse =
        AwsS3ImageGenerator.GenerateSuccessS3ImageResponse();

    public UserImageSubmissionServiceTests()
    {
        IImageProcessingService imageProcessingServiceMock = Substitute.For<IImageProcessingService>();
        _fileUploadClientMock = Substitute.For<IFileUploadClient>();
        IOptions<ImagesData> imagesData = Options.Create(new ImagesData()
        {
            DefaultUserProfilePicture = "DefaultProfilePicture"
        });
        var loggerMock = Substitute.For<ILogger<UserImageSubmissionService>>();
        _sut = new UserImageSubmissionService(
            imageProcessingServiceMock,
            _fileUploadClientMock,
            imagesData,
            loggerMock);
    }

    [Fact]
    public async Task Failed_User_Image_Upload_Throws_InternalServerErrorException()
    {
        _fileUploadClientMock
            .UploadUserImageAsync(Arg.Any<MemoryStream>(), Constants.UserData.ImageFile, Arg.Any<Guid>())
            .Returns(S3FailFileResponse);

        async Task Result() => await _sut.UploadUserImageAsync(User.Id, Constants.UserData.ImageFile, null);

        var exception = await Assert.ThrowsAsync<InternalServerErrorException>(Result);
        Assert.Equal("Não foi possível fazer upload da imagem, tente novamente mais tarde.", exception.Message);
    }

    [Fact]
    public async Task User_Image_Upload_Returns_Uploaded_Image_Url()
    {
        _fileUploadClientMock
            .UploadUserImageAsync(Arg.Any<MemoryStream>(), EditUserRequest.Image,
                Arg.Any<Guid>())
            .Returns(S3SuccessFileResponse);

        string uploadedUrl = await _sut.UploadUserImageAsync(User.Id, EditUserRequest.Image, null);

        Assert.Equal(S3SuccessFileResponse.PublicUrl, uploadedUrl);
    }
}
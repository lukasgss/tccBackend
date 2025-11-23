using Application.Commands.Users.Images.Upload;
using Application.Common.Exceptions;
using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.General.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tests.Users;

public sealed class UploadUserImageCommandHandlerTests
{
	private readonly IImageProcessingService _imageProcessingService;
	private readonly IFileUploadClient _fileUploadClient;
	private readonly UploadUserImageCommandHandler _handler;

	public UploadUserImageCommandHandlerTests()
	{
		_imageProcessingService = Substitute.For<IImageProcessingService>();
		_fileUploadClient = Substitute.For<IFileUploadClient>();
		var logger = Substitute.For<ILogger<UploadUserImageCommandHandler>>();

		ImagesData imagesData = new() { DefaultUserProfilePicture = "default-profile.jpg" };
		var options = Substitute.For<IOptions<ImagesData>>();
		options.Value.Returns(imagesData);

		_handler = new UploadUserImageCommandHandler(
			_imageProcessingService,
			_fileUploadClient,
			options,
			logger);
	}

	[Fact]
	public async Task Handle_WhenUploadFails_ShouldThrowInternalServerErrorException()
	{
		// Arrange
		var userImage = Substitute.For<IFormFile>();
		userImage.OpenReadStream().Returns(new MemoryStream());
		var command = new UploadUserImageCommand(userImage, null);

		_imageProcessingService.CompressImageAsync(Arg.Any<Stream>())
			.Returns(new MemoryStream());
		_fileUploadClient.UploadUserImageAsync(Arg.Any<MemoryStream?>(), Arg.Any<IFormFile?>(), Arg.Any<Guid>())
			.Returns(new AwsS3FileResponse { Success = false, PublicUrl = null });

		// Act & Assert
		var exception =
			await Should.ThrowAsync<InternalServerErrorException>(() =>
				_handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível fazer upload da imagem, tente novamente mais tarde.");
	}

	[Fact]
	public async Task Handle_WhenUploadSucceedsWithNoPreviousImage_ShouldReturnUrl()
	{
		// Arrange
		var userImage = Substitute.For<IFormFile>();
		userImage.OpenReadStream().Returns(new MemoryStream());
		var command = new UploadUserImageCommand(userImage, null);
		var uploadedUrl = "http://uploaded-image.jpg";

		_imageProcessingService.CompressImageAsync(Arg.Any<Stream>())
			.Returns(new MemoryStream());
		_fileUploadClient.UploadUserImageAsync(Arg.Any<MemoryStream?>(), Arg.Any<IFormFile?>(), Arg.Any<Guid>())
			.Returns(new AwsS3FileResponse { Success = true, PublicUrl = uploadedUrl });

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(uploadedUrl);
		await _fileUploadClient.DidNotReceive().DeletePreviousUserImageAsync(Arg.Any<string>());
	}

	[Fact]
	public async Task Handle_WhenPreviousImageIsDefault_ShouldNotDeleteIt()
	{
		// Arrange
		var userImage = Substitute.For<IFormFile>();
		userImage.OpenReadStream().Returns(new MemoryStream());
		var previousImageUrl = "http://images/default-profile.jpg";
		var command = new UploadUserImageCommand(userImage, previousImageUrl);
		var uploadedUrl = "http://uploaded-image.jpg";

		_imageProcessingService.CompressImageAsync(Arg.Any<Stream>())
			.Returns(new MemoryStream());
		_fileUploadClient.UploadUserImageAsync(Arg.Any<MemoryStream?>(), Arg.Any<IFormFile?>(), Arg.Any<Guid>())
			.Returns(new AwsS3FileResponse { Success = true, PublicUrl = uploadedUrl });

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(uploadedUrl);
		await _fileUploadClient.DidNotReceive().DeletePreviousUserImageAsync(Arg.Any<string>());
	}

	[Fact]
	public async Task Handle_WhenPreviousImageIsNotDefault_ShouldDeleteIt()
	{
		// Arrange
		var userImage = Substitute.For<IFormFile>();
		userImage.OpenReadStream().Returns(new MemoryStream());
		var previousImageUrl = "http://images/custom-image.jpg";
		var command = new UploadUserImageCommand(userImage, previousImageUrl);
		var uploadedUrl = "http://uploaded-image.jpg";

		_imageProcessingService.CompressImageAsync(Arg.Any<Stream>())
			.Returns(new MemoryStream());
		_fileUploadClient.UploadUserImageAsync(Arg.Any<MemoryStream?>(), Arg.Any<IFormFile?>(), Arg.Any<Guid>())
			.Returns(new AwsS3FileResponse { Success = true, PublicUrl = uploadedUrl });

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(uploadedUrl);
		await _fileUploadClient.Received(1).DeletePreviousUserImageAsync(previousImageUrl);
	}

	[Fact]
	public async Task Handle_WhenUserImageIsNull_ShouldStillAttemptUpload()
	{
		// Arrange
		var command = new UploadUserImageCommand(null, null);
		var uploadedUrl = "http://uploaded-image.jpg";

		_fileUploadClient.UploadUserImageAsync(null, null, Arg.Any<Guid>())
			.Returns(new AwsS3FileResponse { Success = true, PublicUrl = uploadedUrl });

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(uploadedUrl);
		await _imageProcessingService.DidNotReceive().CompressImageAsync(Arg.Any<Stream>());
	}
}
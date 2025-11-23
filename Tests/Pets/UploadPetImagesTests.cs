using Application.Commands.Pets.UploadImages;
using Application.Common.Interfaces.General.Files;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Tests.Pets;

public sealed class UploadPetImagesCommandHandlerTests
{
	private readonly IPetImageSubmissionService _petImageSubmissionService;
	private readonly UploadPetImagesCommandHandler _handler;

	public UploadPetImagesCommandHandlerTests()
	{
		_petImageSubmissionService = Substitute.For<IPetImageSubmissionService>();

		_handler = new UploadPetImagesCommandHandler(_petImageSubmissionService);
	}

	[Fact]
	public async Task Handle_WhenImagesProvided_ShouldUploadAndReturnPetImages()
	{
		// Arrange
		var petId = Guid.NewGuid();
		var pet = CreatePet(petId);
		var images = new List<IFormFile>
		{
			Substitute.For<IFormFile>(),
			Substitute.For<IFormFile>()
		};
		var command = new UploadPetImagesCommand(pet, images);

		var uploadedUrls = new List<string>
		{
			"http://image1.jpg",
			"http://image2.jpg"
		};
		_petImageSubmissionService.UploadPetImageAsync(images).Returns(uploadedUrls);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		result.Count.ShouldBe(2);
		result[0].ImageUrl.ShouldBe("http://image1.jpg");
		result[0].PetId.ShouldBe(petId);
		result[0].Pet.ShouldBe(pet);
		result[1].ImageUrl.ShouldBe("http://image2.jpg");
		result[1].PetId.ShouldBe(petId);
		result[1].Pet.ShouldBe(pet);
	}

	[Fact]
	public async Task Handle_WhenNoImages_ShouldReturnEmptyList()
	{
		// Arrange
		var petId = Guid.NewGuid();
		var pet = CreatePet(petId);
		var images = new List<IFormFile>();
		var command = new UploadPetImagesCommand(pet, images);

		_petImageSubmissionService.UploadPetImageAsync(images).Returns(new List<string>());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		result.ShouldBeEmpty();
	}

	private static Pet CreatePet(Guid petId)
	{
		return new Pet
		{
			Id = petId,
			Name = "Test Pet",
			Age = Age.Adulto,
			Size = Size.Médio,
			Gender = Gender.Macho,
			Images = new List<PetImage>(),
			Colors = new List<Color>(),
			Species = new Species { Id = 1, Name = "Test Species" },
			Breed = new Breed { Id = 1, Name = "Test Breed" }
		};
	}
}
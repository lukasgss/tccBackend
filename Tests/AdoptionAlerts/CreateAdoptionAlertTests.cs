using Application.Commands.AdoptionAlerts.CreateAdoptionAlert;
using Application.Commands.Alerts.Common;
using Application.Commands.Pets.UploadImages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Color = Domain.Entities.Color;
using Size = Domain.Enums.Size;

namespace Tests.AdoptionAlerts;

public sealed class CreateAdoptionAlertCommandHandlerTests
{
	private readonly IUserRepository _userRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;
	private readonly ISender _mediator;
	private readonly IValueProvider _valueProvider;
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IAdoptionAlertFileSubmissionService _adoptionAlertFileSubmissionService;
	private readonly ILocationUtils _locationUtils;
	private readonly CreateAdoptionAlertCommandHandler _handler;

	public CreateAdoptionAlertCommandHandlerTests()
	{
		_userRepository = Substitute.For<IUserRepository>();
		ILocalizationRepository localizationRepository = Substitute.For<ILocalizationRepository>();
		_breedRepository = Substitute.For<IBreedRepository>();
		_speciesRepository = Substitute.For<ISpeciesRepository>();
		_colorRepository = Substitute.For<IColorRepository>();
		_mediator = Substitute.For<ISender>();
		_valueProvider = Substitute.For<IValueProvider>();
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_adoptionAlertFileSubmissionService = Substitute.For<IAdoptionAlertFileSubmissionService>();
		_locationUtils = Substitute.For<ILocationUtils>();
		var logger = Substitute.For<ILogger<CreateAdoptionAlertCommandHandler>>();

		_handler = new CreateAdoptionAlertCommandHandler(
			_mediator,
			_valueProvider,
			_adoptionAlertRepository,
			logger,
			_userRepository,
			localizationRepository,
			_breedRepository,
			_speciesRepository,
			_colorRepository,
			_adoptionAlertFileSubmissionService,
			_locationUtils
		);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var command = CreateValidCommand(userId);

		_userRepository.GetUserByIdAsync(userId).Returns((User?)null);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		var exception = await act.ShouldThrowAsync<NotFoundException>();
		exception.Message.ShouldBe("Usuário com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenBreedNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var user = new User { Id = userId };
		var command = CreateValidCommand(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns((Breed?)null);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		var exception = await act.ShouldThrowAsync<NotFoundException>();
		exception.Message.ShouldBe("Raça especificada não existe.");
	}

	[Fact]
	public async Task Handle_WhenSpeciesNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var user = new User { Id = userId };
		var breed = new Breed { Id = 1 };
		var command = CreateValidCommand(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns(breed);
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>()).Returns((Species?)null);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		var exception = await act.ShouldThrowAsync<NotFoundException>();
		exception.Message.ShouldBe("Espécie especificada não existe.");
	}

	[Fact]
	public async Task Handle_WhenColorNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var user = new User { Id = userId };
		var breed = new Breed { Id = 1 };
		var species = new Species { Id = 1 };
		var command = CreateValidCommand(userId);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns(breed);
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>()).Returns(species);
		_colorRepository.GetMultipleColorsByIdsAsync(Arg.Any<List<int>>()).Returns(new List<Color>());

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		var exception = await act.ShouldThrowAsync<NotFoundException>();
		exception.Message.ShouldBe("Alguma das cores especificadas não existe.");
	}

	[Fact]
	public async Task Handle_WhenColorCountMismatch_ShouldThrowNotFoundException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var user = new User { Id = userId };
		var breed = new Breed { Id = 1 };
		var species = new Species { Id = 1 };
		var colors = new List<Color> { new Color { Id = 1 } };

		var createPetRequest = new CreatePetRequest(
			Name: "Buddy",
			Gender: Gender.Macho,
			Size: Size.Médio,
			Age: Age.Adulto,
			IsCastrated: true,
			IsVaccinated: true,
			IsNegativeToFivFelv: false,
			IsNegativeToLeishmaniasis: false,
			Images: new List<IFormFile>(),
			BreedId: 1,
			SpeciesId: 1,
			ColorIds: new List<int> { 1, 2 }
		);

		var command = new CreateAdoptionAlertCommand(
			userId, new List<string>(), "Neighborhood", 1, 1, null,
			createPetRequest, false, null, false
		);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns(breed);
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>()).Returns(species);
		_colorRepository.GetMultipleColorsByIdsAsync(Arg.Any<List<int>>()).Returns(colors);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		var exception = await act.ShouldThrowAsync<NotFoundException>();
		exception.Message.ShouldBe("Alguma das cores especificadas não existe.");
	}

	[Fact]
	public async Task Handle_WhenImageCountIs10OrMore_ShouldThrowBadRequestException()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var user = new User { Id = userId };
		var breed = new Breed { Id = 1 };
		var species = new Species { Id = 1 };
		var colors = new List<Color> { new Color { Id = 1 } };

		var images = Enumerable.Range(0, 10)
			.Select(_ => Substitute.For<IFormFile>())
			.ToList();

		var createPetRequest = new CreatePetRequest(
			Name: "Buddy",
			Gender: Gender.Macho,
			Size: Size.Médio,
			Age: Age.Adulto,
			IsCastrated: true,
			IsVaccinated: true,
			IsNegativeToFivFelv: false,
			IsNegativeToLeishmaniasis: false,
			Images: images,
			BreedId: 1,
			SpeciesId: 1,
			ColorIds: new List<int> { 1 }
		);

		var command = new CreateAdoptionAlertCommand(
			userId, new List<string>(), "Neighborhood", 1, 1, null,
			createPetRequest, false, null, false
		);

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns(breed);
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>()).Returns(species);
		_colorRepository.GetMultipleColorsByIdsAsync(Arg.Any<List<int>>()).Returns(colors);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		var exception = await act.ShouldThrowAsync<BadRequestException>();
		exception.Message.ShouldBe("Não é possível adicionar 10 ou mais imagens");
	}

	[Fact]
	public async Task Handle_WithForceCreationWithNotFoundCoordinates_ShouldNotGetLocation()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var command = CreateValidCommand(userId) with
		{
			ForceCreationWithNotFoundCoordinates = true
		};

		var state = new State(1, "State");
		var city = new City(1, "City");
		var localization = new AlertLocalization
		{
			State = state,
			City = city
		};

		_locationUtils.GetAlertStateAndCity(Arg.Any<int>(), Arg.Any<int>()).Returns(localization);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _locationUtils.DidNotReceive().GetAlertLocation(Arg.Any<AlertLocalization>(), Arg.Any<string>());
	}

	[Fact]
	public async Task Handle_WithAdoptionForm_ShouldUploadFormAndCreateFileAttachment()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var adoptionForm = Substitute.For<IFormFile>();
		adoptionForm.FileName.Returns("form.pdf");

		var command = CreateValidCommand(userId) with
		{
			AdoptionForm = adoptionForm,
			ShouldUseDefaultAdoptionForm = false
		};

		_adoptionAlertFileSubmissionService
			.UploadAdoptionFormAsync(adoptionForm, null, null, false)
			.Returns("https://example.com/form.pdf");

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _adoptionAlertFileSubmissionService.Received(1)
			.UploadAdoptionFormAsync(adoptionForm, null, null, false);

		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.AdoptionForm != null &&
			a.AdoptionForm.FileUrl == "https://example.com/form.pdf" &&
			a.AdoptionForm.FileName == "form.pdf"
		));
	}

	[Fact]
	public async Task Handle_WithoutAdoptionForm_ShouldNotCreateFileAttachment()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var command = CreateValidCommand(userId) with
		{
			AdoptionForm = null,
			ShouldUseDefaultAdoptionForm = false
		};

		_adoptionAlertFileSubmissionService
			.UploadAdoptionFormAsync(null, null, null, false)
			.Returns((string?)null);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.AdoptionForm == null
		));
	}

	[Fact]
	public async Task Handle_ShouldFormatAdoptionRestrictionsCorrectly()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var command = CreateValidCommand(userId) with
		{
			AdoptionRestrictions = new List<string> { "  must have yard  ", "no other pets" }
		};

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.AdoptionRestrictions[0] == "Must have yard" &&
			a.AdoptionRestrictions[1] == "No other pets"
		));
	}

	[Fact]
	public async Task Handle_ShouldUploadPetImages()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var petImages = new List<PetImage>
		{
			new PetImage { Id = 1, ImageUrl = "image1.jpg" },
			new PetImage { Id = 2, ImageUrl = "image2.jpg" }
		};

		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(petImages);

		var command = CreateValidCommand(userId);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _mediator.Received(1).Send(
			Arg.Any<UploadPetImagesCommand>(),
			Arg.Any<CancellationToken>()
		);

		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.Pet.Images.Count == 2 &&
			a.Pet.Images.Any(img => img.ImageUrl == "image1.jpg") &&
			a.Pet.Images.Any(img => img.ImageUrl == "image2.jpg")
		));
	}

	[Theory]
	[InlineData(Gender.Desconhecido)]
	[InlineData(Gender.Macho)]
	[InlineData(Gender.Fêmea)]
	public async Task Handle_ShouldHandleAllGenderEnumValues(Gender gender)
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var createPetRequest = new CreatePetRequest(
			Name: "Pet",
			Gender: gender,
			Size: Size.Médio,
			Age: Age.Adulto,
			IsCastrated: true,
			IsVaccinated: true,
			IsNegativeToFivFelv: false,
			IsNegativeToLeishmaniasis: false,
			Images: new List<IFormFile>(),
			BreedId: 1,
			SpeciesId: 1,
			ColorIds: new List<int> { 1 }
		);

		var command = CreateValidCommand(userId) with { Pet = createPetRequest };

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.Pet.Gender == gender
		));
	}

	[Theory]
	[InlineData(Age.Filhote)]
	[InlineData(Age.Jovem)]
	[InlineData(Age.Adulto)]
	[InlineData(Age.Sênior)]
	public async Task Handle_ShouldHandleAllAgeEnumValues(Age age)
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var createPetRequest = new CreatePetRequest(
			Name: "Pet",
			Gender: Gender.Macho,
			Size: Size.Médio,
			Age: age,
			IsCastrated: true,
			IsVaccinated: true,
			IsNegativeToFivFelv: false,
			IsNegativeToLeishmaniasis: false,
			Images: new List<IFormFile>(),
			BreedId: 1,
			SpeciesId: 1,
			ColorIds: new List<int> { 1 }
		);

		var command = CreateValidCommand(userId) with { Pet = createPetRequest };

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.Pet.Age == age
		));
	}

	[Fact]
	public async Task Handle_WithNullablePetProperties_ShouldHandleNullValues()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var createPetRequest = new CreatePetRequest(
			Name: "Pet",
			Gender: Gender.Desconhecido,
			Size: Size.Médio,
			Age: Age.Adulto,
			IsCastrated: null,
			IsVaccinated: null,
			IsNegativeToFivFelv: null,
			IsNegativeToLeishmaniasis: null,
			Images: new List<IFormFile>(),
			BreedId: 1,
			SpeciesId: 1,
			ColorIds: new List<int> { 1 }
		);

		var command = CreateValidCommand(userId) with { Pet = createPetRequest };

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.Pet.IsCastrated == null &&
			a.Pet.IsVaccinated == null &&
			a.Pet.IsNegativeToFivFelv == null &&
			a.Pet.IsNegativeToLeishmaniasis == null
		));
	}

	[Fact]
	public async Task Handle_WithMultipleColors_ShouldAssignAllColors()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var user = new User { Id = userId };
		var breed = new Breed { Id = 1, Name = "Labrador" };
		var species = new Species { Id = 1, Name = "Dog" };
		var colors = new List<Color>
		{
			new Color { Id = 1, Name = "Brown", HexCode = "#8B4513" },
			new Color { Id = 2, Name = "White", HexCode = "#FFFFFF" },
			new Color { Id = 3, Name = "Black", HexCode = "#000000" }
		};

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns(breed);
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>()).Returns(species);
		_colorRepository.GetMultipleColorsByIdsAsync(Arg.Any<List<int>>()).Returns(colors);
		SetupLocationAndOtherDependencies();

		var createPetRequest = new CreatePetRequest(
			Name: "Pet",
			Gender: Gender.Macho,
			Size: Size.Médio,
			Age: Age.Adulto,
			IsCastrated: true,
			IsVaccinated: true,
			IsNegativeToFivFelv: false,
			IsNegativeToLeishmaniasis: false,
			Images: new List<IFormFile>(),
			BreedId: 1,
			SpeciesId: 1,
			ColorIds: new List<int> { 1, 2, 3 }
		);

		var command = CreateValidCommand(userId) with { Pet = createPetRequest };

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.Pet.Colors.Count == 3 &&
			a.Pet.Colors.Any(c => c.Id == 1) &&
			a.Pet.Colors.Any(c => c.Id == 2) &&
			a.Pet.Colors.Any(c => c.Id == 3)
		));
	}

	[Fact]
	public async Task Handle_WithEmptyAdoptionRestrictions_ShouldSucceed()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var command = CreateValidCommand(userId) with
		{
			AdoptionRestrictions = new List<string>()
		};

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.AdoptionRestrictions.Count == 0
		));
	}

	[Fact]
	public async Task Handle_WithNullDescription_ShouldSucceed()
	{
		// Arrange
		var userId = Guid.NewGuid();
		SetupValidRepositories(userId);

		var command = CreateValidCommand(userId) with
		{
			Description = null
		};

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Add(Arg.Is<AdoptionAlert>(a =>
			a.Description == null
		));
	}

	private CreateAdoptionAlertCommand CreateValidCommand(Guid userId)
	{
		return new CreateAdoptionAlertCommand(
			UserId: userId,
			AdoptionRestrictions: new List<string> { "Must have yard" },
			Neighborhood: "Downtown",
			State: 1,
			City: 1,
			Description: "Friendly pet",
			Pet: new CreatePetRequest(
				Name: "Buddy",
				Gender: Gender.Macho,
				Size: Size.Médio,
				Age: Age.Adulto,
				IsCastrated: true,
				IsVaccinated: true,
				IsNegativeToFivFelv: false,
				IsNegativeToLeishmaniasis: false,
				Images: new List<IFormFile>(),
				BreedId: 1,
				SpeciesId: 1,
				ColorIds: new List<int> { 1 }
			),
			ForceCreationWithNotFoundCoordinates: false,
			AdoptionForm: null,
			ShouldUseDefaultAdoptionForm: false
		);
	}

	private void SetupValidRepositories(Guid userId)
	{
		var user = new User { Id = userId };
		var breed = new Breed { Id = 1, Name = "Labrador" };
		var species = new Species { Id = 1, Name = "Dog" };
		var colors = new List<Color> { new Color { Id = 1, Name = "Brown", HexCode = "#8B4513" } };

		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>()).Returns(breed);
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>()).Returns(species);
		_colorRepository.GetMultipleColorsByIdsAsync(Arg.Any<List<int>>()).Returns(colors);
		SetupLocationAndOtherDependencies();
	}

	private void SetupLocationAndOtherDependencies()
	{
		var state = new State(1, "State");
		var city = new City(1, "City");
		var localization = new AlertLocalization
		{
			State = state,
			City = city
		};
		var location = new Point(10, 20);
		var petImages = new List<PetImage>();

		_locationUtils.GetAlertStateAndCity(Arg.Any<int>(), Arg.Any<int>()).Returns(localization);
		_locationUtils.GetAlertLocation(Arg.Any<AlertLocalization>(), Arg.Any<string>()).Returns(location);
		_valueProvider.NewGuid().Returns(Guid.NewGuid(), Guid.NewGuid());
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>()).Returns(petImages);
		_adoptionAlertFileSubmissionService
			.UploadAdoptionFormAsync(Arg.Any<IFormFile?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>())
			.Returns((string?)null);
	}
}
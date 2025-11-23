using Application.Commands.AdoptionAlerts.Update;
using Application.Commands.Pets.UploadImages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.Localization;
using Application.Queries.GeoLocation.Common;
using Application.Queries.GeoLocation.GetCoordinatesFromStateAndCity;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Tests.AdoptionAlerts;

public sealed class UpdateAdoptionAlertCommandHandlerTests
{
	private readonly ISender _mediator;
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IPetImageRepository _petImageRepository;
	private readonly IPetImageSubmissionService _petImageSubmissionService;
	private readonly ILocalizationRepository _localizationRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;
	private readonly IAdoptionAlertFileSubmissionService _adoptionAlertFileSubmissionService;
	private readonly UpdateAdoptionAlertCommandHandler _handler;

	public UpdateAdoptionAlertCommandHandlerTests()
	{
		_mediator = Substitute.For<ISender>();
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_petImageRepository = Substitute.For<IPetImageRepository>();
		_petImageSubmissionService = Substitute.For<IPetImageSubmissionService>();
		_localizationRepository = Substitute.For<ILocalizationRepository>();
		_breedRepository = Substitute.For<IBreedRepository>();
		_speciesRepository = Substitute.For<ISpeciesRepository>();
		_colorRepository = Substitute.For<IColorRepository>();
		_adoptionAlertFileSubmissionService = Substitute.For<IAdoptionAlertFileSubmissionService>();
		var logger = Substitute.For<ILogger<UpdateAdoptionAlertCommandHandler>>();

		_handler = new UpdateAdoptionAlertCommandHandler(
			_mediator,
			_adoptionAlertRepository,
			_petImageRepository,
			_petImageSubmissionService,
			_localizationRepository,
			_breedRepository,
			logger,
			_speciesRepository,
			_colorRepository,
			_adoptionAlertFileSubmissionService);
	}

	[Fact]
	public async Task Handle_WhenAdoptionAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns((AdoptionAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alerta de adoção com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldThrowUnauthorizedException()
	{
		// Arrange
		var command = CreateValidCommand();
		var adoptionAlert = CreateAdoptionAlert(ownerId: Guid.NewGuid());
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível alterar alertas de adoção de outros usuários.");
	}

	[Fact]
	public async Task Handle_WhenStateNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_localizationRepository.GetStateById(command.State).Returns((State?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Estado especificado não foi encontrado.");
	}

	[Fact]
	public async Task Handle_WhenCityNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_localizationRepository.GetStateById(command.State).Returns(new State(1, "Test State"));
		_localizationRepository.GetCityById(command.City).Returns((City?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Cidade especificada não foi encontrada.");
	}

	[Fact]
	public async Task Handle_WhenCoordinatesNotFoundAndNotForced_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: false);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_mediator.Send(Arg.Any<GetCoordinatesFromStateAndCityQuery>(), Arg.Any<CancellationToken>())
			.Returns((GeoLocationResponse?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não foi possível encontrar coordenadas do bairro especificado.");
	}

	[Fact]
	public async Task Handle_WhenBreedNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: true);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		SetupValidAdoptionFormUpload();
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_breedRepository.GetBreedByIdAsync(command.Pet.BreedId).Returns((Breed?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Raça especificada não existe.");
	}

	[Fact]
	public async Task Handle_WhenSpeciesNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: true);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		SetupValidAdoptionFormUpload();
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_breedRepository.GetBreedByIdAsync(command.Pet.BreedId).Returns(new Breed { Id = 1, Name = "Test Breed" });
		_speciesRepository.GetSpeciesByIdAsync(command.Pet.SpeciesId).Returns((Species?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Espécie especificada não existe.");
	}

	[Fact]
	public async Task Handle_WhenColorsNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: true);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		SetupValidAdoptionFormUpload();
		SetupValidPetDependencies();
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_petImageRepository.GetImagesFromPetByIdAsync(Arg.Any<Guid>()).Returns([]);
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(new List<PetImage>());
		_colorRepository.GetMultipleColorsByIdsAsync(command.Pet.ColorIds).Returns([]);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alguma das cores especificadas não existe.");
	}

	[Fact]
	public async Task Handle_WhenValidRequestWithForceCreation_ShouldUpdateAndReturnResponse()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: true);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		SetupValidAdoptionFormUpload();
		SetupValidPetDependencies();
		SetupValidColors(command.Pet.ColorIds);
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_petImageRepository.GetImagesFromPetByIdAsync(Arg.Any<Guid>()).Returns(new List<PetImage>());
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(new List<PetImage>());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		await _adoptionAlertRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenValidRequestWithCoordinates_ShouldUpdateLocationAndReturnResponse()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: false);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		SetupValidAdoptionFormUpload();
		SetupValidPetDependencies();
		SetupValidColors(command.Pet.ColorIds);
		SetupValidCoordinates();
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_petImageRepository.GetImagesFromPetByIdAsync(Arg.Any<Guid>()).Returns(new List<PetImage>());
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(new List<PetImage>());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		adoptionAlert.Location.ShouldNotBeNull();
		await _adoptionAlertRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenExistingAdoptionFormUrlProvided_ShouldNotUploadNewForm()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: true, existingFormUrl: "http://existing-form.pdf");
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		SetupValidLocalization();
		SetupValidPetDependencies();
		SetupValidColors(command.Pet.ColorIds);
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_petImageRepository.GetImagesFromPetByIdAsync(Arg.Any<Guid>()).Returns(new List<PetImage>());
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(new List<PetImage>());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		await _adoptionAlertFileSubmissionService.DidNotReceive()
			.UploadAdoptionFormAsync(Arg.Any<IFormFile?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>());
	}

	[Fact]
	public async Task Handle_WhenPetImagesNeedDeletion_ShouldDeleteRemovedImages()
	{
		// Arrange
		var command = CreateValidCommand(forceCreation: true);
		var adoptionAlert = CreateAdoptionAlert(ownerId: command.UserId);
		var existingImages = new List<PetImage>
		{
			new() { Id = 1, ImageUrl = "http://image1.jpg" },
			new() { Id = 2, ImageUrl = "http://image2.jpg" }
		};
		SetupValidLocalization();
		SetupValidAdoptionFormUpload();
		SetupValidPetDependencies();
		SetupValidColors(command.Pet.ColorIds);
		_adoptionAlertRepository.GetByIdAsync(command.Id).Returns(adoptionAlert);
		_petImageRepository.GetImagesFromPetByIdAsync(Arg.Any<Guid>()).Returns(existingImages);
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(new List<PetImage>());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		await _petImageSubmissionService.Received(1)
			.DeletePetImageAsync(Arg.Any<Guid>(), Arg.Any<List<PetImage>>());
	}

	private UpdateAdoptionAlertCommand CreateValidCommand(
		bool forceCreation = false,
		string? existingFormUrl = null)
	{
		var formFile = Substitute.For<IFormFile>();
		formFile.FileName.Returns("adoption-form.pdf");
		formFile.Name.Returns("adoption-form.pdf");

		return new UpdateAdoptionAlertCommand(
			Id: Guid.NewGuid(),
			Neighborhood: "Test Neighborhood",
			State: 1,
			City: 1,
			Description: "Test Description",
			Pet: new EditPetRequest(
				"Test Pet",
				Gender.Macho,
				Age.Adulto,
				new List<IFormFile>(),
				new List<string>(),
				true,
				true,
				true,
				true,
				1,
				1,
				Size.Grande,
				new List<int> { 1, 2 }
			),
			UserId: Guid.NewGuid(),
			AdoptionRestrictions: new List<string> { "restriction1", "restriction2" },
			ForceCreationWithNotFoundCoordinates: forceCreation,
			AdoptionForm: forceCreation ? formFile : null,
			ExistingAdoptionFormUrl: existingFormUrl,
			ShouldUseDefaultAdoptionForm: false
		);
	}

	private AdoptionAlert CreateAdoptionAlert(Guid ownerId)
	{
		return new AdoptionAlert
		{
			Id = Guid.NewGuid(),
			User = new User { Id = ownerId, DefaultAdoptionFormUrl = null },
			Pet = new Pet
			{
				Id = Guid.NewGuid(),
				Name = "Original Pet",
				Images = new List<PetImage>(),
				Colors = new List<Color>(),
				Age = Age.Adulto,
				Size = Size.Gigante
			},
			Neighborhood = "Original Neighborhood",
			AdoptionRestrictions = new List<string>(),
			AdoptionForm = null
		};
	}

	private void SetupValidLocalization()
	{
		_localizationRepository.GetStateById(Arg.Any<int>())
			.Returns(new State(1, "Test State"));
		_localizationRepository.GetCityById(Arg.Any<int>())
			.Returns(new City(1, "Test City"));
	}

	private void SetupValidAdoptionFormUpload()
	{
		_adoptionAlertFileSubmissionService
			.UploadAdoptionFormAsync(Arg.Any<IFormFile?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<bool>())
			.Returns("http://uploaded-form.pdf");
	}

	private void SetupValidPetDependencies()
	{
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>())
			.Returns(new Breed { Id = 1, Name = "Test Breed" });
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>())
			.Returns(new Species { Id = 1, Name = "Test Species" });
	}

	private void SetupValidColors(List<int> colorIds)
	{
		var colors = colorIds.Select(id => new Color { Id = id, Name = $"Color {id}" }).ToList();
		_colorRepository.GetMultipleColorsByIdsAsync(colorIds).Returns(colors);
	}

	private void SetupValidCoordinates()
	{
		_mediator.Send(Arg.Any<GetCoordinatesFromStateAndCityQuery>(), Arg.Any<CancellationToken>())
			.Returns(new GeoLocationResponse(Latitude: "-23.5505", Longitude: "-46.6333", null, null, null, null,
				null));
	}
}
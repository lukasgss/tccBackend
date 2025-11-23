using Application.Commands.Alerts.Common;
using Application.Commands.MissingAlerts.Create;
using Application.Commands.Pets.UploadImages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Tests.MissingAlerts;

public sealed class CreateMissingAlertCommandHandlerTests
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;
	private readonly ILocationUtils _locationUtils;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;
	private readonly ISender _mediator;
	private readonly CreateMissingAlertCommandHandler _handler;

	public CreateMissingAlertCommandHandlerTests()
	{
		_missingAlertRepository = Substitute.For<IMissingAlertRepository>();
		_userRepository = Substitute.For<IUserRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		_locationUtils = Substitute.For<ILocationUtils>();
		_breedRepository = Substitute.For<IBreedRepository>();
		_speciesRepository = Substitute.For<ISpeciesRepository>();
		_colorRepository = Substitute.For<IColorRepository>();
		_mediator = Substitute.For<ISender>();
		var logger = Substitute.For<ILogger<CreateMissingAlertCommandHandler>>();

		_handler = new CreateMissingAlertCommandHandler(
			_missingAlertRepository,
			Substitute.For<IPetRepository>(),
			_userRepository,
			_valueProvider,
			logger,
			_locationUtils,
			_breedRepository,
			_speciesRepository,
			_colorRepository,
			_mediator);
	}

	[Fact]
	public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		_userRepository.GetUserByIdAsync(command.UserId).Returns((User?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Usuário com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenBreedNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = CreateValidCommand();
		_userRepository.GetUserByIdAsync(command.UserId).Returns(new User { Id = command.UserId });
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
		var command = CreateValidCommand();
		_userRepository.GetUserByIdAsync(command.UserId).Returns(new User { Id = command.UserId });
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
		var command = CreateValidCommand();
		_userRepository.GetUserByIdAsync(command.UserId).Returns(new User { Id = command.UserId });
		_breedRepository.GetBreedByIdAsync(command.Pet.BreedId).Returns(new Breed { Id = 1, Name = "Test Breed" });
		_speciesRepository.GetSpeciesByIdAsync(command.Pet.SpeciesId)
			.Returns(new Species { Id = 1, Name = "Test Species" });
		_colorRepository.GetMultipleColorsByIdsAsync(command.Pet.ColorIds).Returns(new List<Color>());

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alguma das cores especificadas não existe.");
	}

	[Fact]
	public async Task Handle_WhenTooManyImages_ShouldThrowBadRequestException()
	{
		// Arrange
		var command = CreateCommandWithManyImages(10);
		SetupValidUser(command.UserId);
		SetupValidPetDependencies(command.Pet.ColorIds);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível adicionar 10 ou mais imagens");
	}

	[Fact]
	public async Task Handle_WhenValidRequest_ShouldCreateAlertAndReturnResponse()
	{
		// Arrange
		var command = CreateValidCommand();
		var alertId = Guid.NewGuid();
		var petId = Guid.NewGuid();
		var now = DateTime.UtcNow;

		SetupValidUser(command.UserId);
		SetupValidPetDependencies(command.Pet.ColorIds);
		SetupValidLocation();

		_valueProvider.NewGuid().Returns(alertId, petId);
		_valueProvider.UtcNow().Returns(now);
		_mediator.Send(Arg.Any<UploadPetImagesCommand>(), Arg.Any<CancellationToken>())
			.Returns(new List<PetImage>());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		_missingAlertRepository.Received(1).Add(Arg.Any<MissingAlert>());
		await _missingAlertRepository.Received(1).CommitAsync();
	}

	private CreateMissingAlertCommand CreateValidCommand()
	{
		return new CreateMissingAlertCommand(
			State: 1,
			City: 1,
			Neighborhood: "Test Neighborhood",
			Description: "Test Description",
			Pet: new CreatePetRequest(
				Name: "Test Pet",
				Gender: Gender.Macho,
				Size: Size.Médio,
				Age: Age.Adulto,
				IsCastrated: true,
				IsVaccinated: true,
				IsNegativeToFivFelv: true,
				IsNegativeToLeishmaniasis: true,
				Images: new List<IFormFile>(),
				BreedId: 1,
				SpeciesId: 1,
				ColorIds: new List<int> { 1, 2 }
			),
			UserId: Guid.NewGuid()
		);
	}

	private CreateMissingAlertCommand CreateCommandWithManyImages(int imageCount)
	{
		var images = Enumerable.Range(0, imageCount)
			.Select(_ => Substitute.For<IFormFile>())
			.ToList();

		return new CreateMissingAlertCommand(
			State: 1,
			City: 1,
			Neighborhood: "Test Neighborhood",
			Description: "Test Description",
			Pet: new CreatePetRequest(
				Name: "Test Pet",
				Gender: Gender.Macho,
				Size: Size.Médio,
				Age: Age.Adulto,
				IsCastrated: true,
				IsVaccinated: true,
				IsNegativeToFivFelv: true,
				IsNegativeToLeishmaniasis: true,
				Images: images,
				BreedId: 1,
				SpeciesId: 1,
				ColorIds: new List<int> { 1, 2 }
			),
			UserId: Guid.NewGuid()
		);
	}

	private void SetupValidUser(Guid userId)
	{
		_userRepository.GetUserByIdAsync(userId).Returns(new User { Id = userId });
	}

	private void SetupValidPetDependencies(List<int> colorIds)
	{
		_breedRepository.GetBreedByIdAsync(Arg.Any<int>())
			.Returns(new Breed { Id = 1, Name = "Test Breed" });
		_speciesRepository.GetSpeciesByIdAsync(Arg.Any<int>())
			.Returns(new Species { Id = 1, Name = "Test Species" });

		var colors = colorIds.Select(id => new Color { Id = id, Name = $"Color {id}" }).ToList();
		_colorRepository.GetMultipleColorsByIdsAsync(colorIds).Returns(colors);
	}

	private void SetupValidLocation()
	{
		var localization = new AlertLocalization
		{
			State = new State(1, "Test State"),
			City = new City(1, "Test City")
		};
		var point = new Point(0, 0);

		_locationUtils.GetAlertStateAndCity(Arg.Any<int>(), Arg.Any<int>()).Returns(localization);
		_locationUtils.GetAlertLocation(Arg.Any<AlertLocalization>(), Arg.Any<string>()).Returns(point);
	}
}
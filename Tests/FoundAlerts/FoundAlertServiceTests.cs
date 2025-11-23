using Application.Commands.Alerts.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Application.Services.General.Messages;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Tests.FoundAlerts;

public sealed class FoundAnimalAlertServiceTests
{
	private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly IUserRepository _userRepository;
	private readonly IColorRepository _colorRepository;
	private readonly IFoundAlertImageSubmissionService _imageSubmissionService;
	private readonly IAlertsMessagingService _alertsMessagingService;
	private readonly IValueProvider _valueProvider;
	private readonly ILocationUtils _locationUtils;
	private readonly FoundAnimalAlertService _service;

	public FoundAnimalAlertServiceTests()
	{
		_foundAnimalAlertRepository = Substitute.For<IFoundAnimalAlertRepository>();
		_speciesRepository = Substitute.For<ISpeciesRepository>();
		_breedRepository = Substitute.For<IBreedRepository>();
		_userRepository = Substitute.For<IUserRepository>();
		_colorRepository = Substitute.For<IColorRepository>();
		_imageSubmissionService = Substitute.For<IFoundAlertImageSubmissionService>();
		_alertsMessagingService = Substitute.For<IAlertsMessagingService>();
		_valueProvider = Substitute.For<IValueProvider>();
		_locationUtils = Substitute.For<ILocationUtils>();
		var logger = Substitute.For<ILogger<FoundAnimalAlertService>>();

		_service = new FoundAnimalAlertService(
			_foundAnimalAlertRepository,
			_speciesRepository,
			_breedRepository,
			_userRepository,
			_colorRepository,
			_imageSubmissionService,
			_alertsMessagingService,
			_valueProvider,
			logger,
			_locationUtils);
	}

	#region GetByIdAsync

	[Fact]
	public async Task GetByIdAsync_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns((FoundAnimalAlert?)null);

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.GetByIdAsync(alertId));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task GetByIdAsync_WhenAlertFound_ShouldReturnResponse()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var alert = CreateFoundAnimalAlert(alertId);
		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);

		// Act
		var result = await _service.GetByIdAsync(alertId);

		// Assert
		result.ShouldNotBeNull();
	}

	#endregion

	#region ListFoundAnimalAlerts

	[Fact]
	public async Task ListFoundAnimalAlerts_WhenPageLessThanOne_ShouldThrowBadRequestException()
	{
		// Arrange
		var filters = new FoundAnimalAlertFilters();

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _service.ListFoundAnimalAlerts(filters, 0, 10));
		exception.Message.ShouldBe("Insira um número e tamanho de página maior ou igual a 1.");
	}

	[Fact]
	public async Task ListFoundAnimalAlerts_WhenPageSizeLessThanOne_ShouldThrowBadRequestException()
	{
		// Arrange
		var filters = new FoundAnimalAlertFilters();

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _service.ListFoundAnimalAlerts(filters, 1, 0));
		exception.Message.ShouldBe("Insira um número e tamanho de página maior ou igual a 1.");
	}

	#endregion

	#region CreateAsync

	[Fact]
	public async Task CreateAsync_WhenSpeciesNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var request = CreateFoundAnimalAlertRequest();
		var userId = Guid.NewGuid();

		_speciesRepository.GetSpeciesByIdAsync(request.SpeciesId).Returns((Species?)null);

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.CreateAsync(request, userId));
		exception.Message.ShouldBe("Espécie com o id especificado não existe.");
	}

	[Fact]
	public async Task CreateAsync_WhenColorsNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var request = CreateFoundAnimalAlertRequest();
		var userId = Guid.NewGuid();

		_speciesRepository.GetSpeciesByIdAsync(request.SpeciesId)
			.Returns(new Species { Id = 1, Name = "Test Species" });
		_colorRepository.GetMultipleColorsByIdsAsync(request.ColorIds).Returns(new List<Color>());

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.CreateAsync(request, userId));
		exception.Message.ShouldBe("Alguma das cores especificadas não existe.");
	}

	[Fact]
	public async Task CreateAsync_WhenBreedNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var request = CreateFoundAnimalAlertRequest(breedId: 1);
		var userId = Guid.NewGuid();

		SetupValidSpeciesAndColors(request);
		_breedRepository.GetBreedByIdAsync(1).Returns((Breed?)null);

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.CreateAsync(request, userId));
		exception.Message.ShouldBe("Raça com o id especificado não existe.");
	}

	[Fact]
	public async Task CreateAsync_WhenValidRequest_ShouldCreateAlertAndReturnResponse()
	{
		// Arrange
		const int breedId = 1;
		var request = CreateFoundAnimalAlertRequest(breedId);
		var userId = Guid.NewGuid();
		var alertId = Guid.NewGuid();
		var now = DateTime.UtcNow;

		SetupValidSpeciesAndColors(request);
		SetupValidLocation();
		_userRepository.GetUserByIdAsync(userId).Returns(CreateUser(userId));
		_valueProvider.NewGuid().Returns(alertId);
		_valueProvider.UtcNow().Returns(now);
		_breedRepository.GetBreedByIdAsync(breedId)
			.Returns(new Breed() { Id = 1, Name = "Test breed" });
		_imageSubmissionService.UploadImagesAsync(alertId, request.Images)
			.Returns(new List<string> { "http://image1.jpg" });

		// Act
		var result = await _service.CreateAsync(request, userId);

		// Assert
		result.ShouldNotBeNull();
		_foundAnimalAlertRepository.Received(1).Add(Arg.Any<FoundAnimalAlert>());
		await _foundAnimalAlertRepository.Received(1).CommitAsync();
		_alertsMessagingService.Received(1).PublishFoundAlert(Arg.Any<FoundAnimalAlert>());
	}

	#endregion

	#region EditAsync

	[Fact]
	public async Task EditAsync_WhenRouteIdDoesNotMatch_ShouldThrowBadRequestException()
	{
		// Arrange
		var request = CreateEditFoundAnimalAlertRequest(Guid.NewGuid());
		var userId = Guid.NewGuid();
		var routeId = Guid.NewGuid();

		// Act & Assert
		var exception =
			await Should.ThrowAsync<BadRequestException>(() => _service.EditAsync(request, userId, routeId));
		exception.Message.ShouldBe("Id da rota não coincide com o id especificado.");
	}

	[Fact]
	public async Task EditAsync_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var request = CreateEditFoundAnimalAlertRequest(alertId);
		var userId = Guid.NewGuid();

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns((FoundAnimalAlert?)null);

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.EditAsync(request, userId, alertId));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task EditAsync_WhenUserIsNotOwner_ShouldThrowForbiddenException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var request = CreateEditFoundAnimalAlertRequest(alertId);
		var alert = CreateFoundAnimalAlert(alertId, ownerId);

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);

		// Act & Assert
		var exception = await Should.ThrowAsync<ForbiddenException>(() => _service.EditAsync(request, userId, alertId));
		exception.Message.ShouldBe("Não é possível editar alertas de outros usuários.");
	}

	#endregion

	#region DeleteAsync

	[Fact]
	public async Task DeleteAsync_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns((FoundAnimalAlert?)null);

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.DeleteAsync(alertId, userId));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task DeleteAsync_WhenUserIsNotOwner_ShouldThrowForbiddenException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var alert = CreateFoundAnimalAlert(alertId, ownerId);

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);

		// Act & Assert
		var exception = await Should.ThrowAsync<ForbiddenException>(() => _service.DeleteAsync(alertId, userId));
		exception.Message.ShouldBe("Não é possível excluir alertas de outros usuários.");
	}

	[Fact]
	public async Task DeleteAsync_WhenUserIsOwner_ShouldDeleteAlert()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var alert = CreateFoundAnimalAlert(alertId, userId);

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);

		// Act
		await _service.DeleteAsync(alertId, userId);

		// Assert
		_foundAnimalAlertRepository.Received(1).Delete(alert);
		await _foundAnimalAlertRepository.Received(1).CommitAsync();
	}

	#endregion

	#region ToggleAlertStatus

	[Fact]
	public async Task ToggleAlertStatus_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns((FoundAnimalAlert?)null);

		// Act & Assert
		var exception = await Should.ThrowAsync<NotFoundException>(() => _service.ToggleAlertStatus(alertId, userId));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task ToggleAlertStatus_WhenUserIsNotOwner_ShouldThrowForbiddenException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var alert = CreateFoundAnimalAlert(alertId, ownerId);

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);

		// Act & Assert
		var exception = await Should.ThrowAsync<ForbiddenException>(() => _service.ToggleAlertStatus(alertId, userId));
		exception.Message.ShouldBe("Não é possível alterar o status de alertas de outros usuários.");
	}

	[Fact]
	public async Task ToggleAlertStatus_WhenRecoveryDateIsNull_ShouldSetRecoveryDate()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var alert = CreateFoundAnimalAlert(alertId, userId);
		alert.RecoveryDate = null;
		var today = DateOnly.FromDateTime(DateTime.UtcNow);

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_valueProvider.DateOnlyNow().Returns(today);

		// Act
		var result = await _service.ToggleAlertStatus(alertId, userId);

		// Assert
		result.ShouldNotBeNull();
		alert.RecoveryDate.ShouldBe(today);
		await _foundAnimalAlertRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task ToggleAlertStatus_WhenRecoveryDateIsSet_ShouldClearRecoveryDate()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var alert = CreateFoundAnimalAlert(alertId, userId);
		alert.RecoveryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));

		_foundAnimalAlertRepository.GetByIdAsync(alertId).Returns(alert);

		// Act
		var result = await _service.ToggleAlertStatus(alertId, userId);

		// Assert
		result.ShouldNotBeNull();
		alert.RecoveryDate.ShouldBeNull();
		await _foundAnimalAlertRepository.Received(1).CommitAsync();
	}

	#endregion

	#region Helper Methods

	private static CreateFoundAnimalAlertRequest CreateFoundAnimalAlertRequest(int? breedId = null)
	{
		return new CreateFoundAnimalAlertRequest
		{
			Name = "Test Animal",
			Description = "Test Description",
			State = 1,
			City = 1,
			Neighborhood = "Test Neighborhood",
			Size = Size.Médio,
			Gender = Gender.Macho,
			Age = Age.Adulto,
			SpeciesId = 1,
			BreedId = breedId,
			ColorIds = new List<int> { 1, 2 },
			Images = new List<IFormFile>()
		};
	}

	private static EditFoundAnimalAlertRequest CreateEditFoundAnimalAlertRequest(Guid alertId)
	{
		return new EditFoundAnimalAlertRequest
		{
			Id = alertId,
			Name = "Updated Animal",
			Description = "Updated Description",
			FoundLocationLatitude = -23.5505,
			FoundLocationLongitude = -46.6333,
			Size = Size.Médio,
			Gender = Gender.Macho,
			Age = Age.Adulto,
			SpeciesId = 1,
			BreedId = null,
			ColorIds = new List<int> { 1, 2 },
			Images = new List<IFormFile>()
		};
	}

	private static FoundAnimalAlert CreateFoundAnimalAlert(Guid alertId, Guid? ownerId = null)
	{
		var actualOwnerId = ownerId ?? Guid.NewGuid();
		return new FoundAnimalAlert
		{
			Id = alertId,
			Name = "Test Animal",
			Description = "Test Description",
			Location = new Point(0, 0),
			Neighborhood = "Test Neighborhood",
			Age = Age.Adulto,
			Size = Size.Médio,
			Gender = Gender.Macho,
			Breed = new Breed() { Id = 1, Name = "test" },
			RegistrationDate = DateTime.UtcNow,
			RecoveryDate = null,
			Images = new List<FoundAnimalAlertImage>(),
			Colors = new List<Color>(),
			Species = new Species { Id = 1, Name = "Test Species" },
			State = new State(1, "Test State"),
			City = new City(1, "Test City"),
			User = CreateUser(actualOwnerId)
		};
	}

	private static User CreateUser(Guid userId)
	{
		return new User
		{
			Id = userId,
			Email = "user@email.com",
			ReceivesOnlyWhatsAppMessages = true,
			PhoneNumber = "123456789",
			FullName = "Test User",
			Image = "image.jpg",
			DefaultAdoptionFormUrl = "url"
		};
	}

	private void SetupValidSpeciesAndColors(CreateFoundAnimalAlertRequest request)
	{
		_speciesRepository.GetSpeciesByIdAsync(request.SpeciesId)
			.Returns(new Species { Id = 1, Name = "Test Species" });
		var colors = request.ColorIds.Select(id => new Color { Id = id, Name = $"Color {id}" }).ToList();
		_colorRepository.GetMultipleColorsByIdsAsync(request.ColorIds).Returns(colors);
	}

	private void SetupValidLocation()
	{
		var localization = new AlertLocalization
		{
			State = new State(1, "Test State"),
			City = new City(1, "Test City")
		};
		_locationUtils.GetAlertStateAndCity(Arg.Any<int>(), Arg.Any<int>()).Returns(localization);
		_locationUtils.GetAlertLocation(Arg.Any<AlertLocalization>(), Arg.Any<string>())
			.Returns(new Point(0, 0));
	}

	#endregion
}
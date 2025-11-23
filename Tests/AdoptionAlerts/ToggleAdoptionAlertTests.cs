using Application.Commands.AdoptionAlerts.ToggleAdoption;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace Tests.AdoptionAlerts;

public sealed class ToggleAdoptionCommandHandlerTests
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IValueProvider _valueProvider;
	private readonly ILogger<ToggleAdoptionAlertCommandHandler> _logger;
	private readonly ToggleAdoptionAlertCommandHandler _handler;

	public ToggleAdoptionCommandHandlerTests()
	{
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		_logger = Substitute.For<ILogger<ToggleAdoptionAlertCommandHandler>>();

		_handler = new ToggleAdoptionAlertCommandHandler(
			_adoptionAlertRepository,
			_valueProvider,
			_logger);
	}

	[Fact]
	public async Task Handle_WhenAdoptionDateIsNullAndUserIsOwner_ShouldSetAdoptionDateAndReturnResponse()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var today = DateOnly.FromDateTime(DateTime.Today);
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_valueProvider.DateOnlyNow().Returns(today);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		adoptionAlert.AdoptionDate.ShouldBe(today);
		await _adoptionAlertRepository.Received(1).CommitAsync();
		_valueProvider.Received(1).DateOnlyNow();

		result.ShouldNotBeNull();
		result.Id.ShouldBe(alertId);
		result.AdoptionDate.ShouldBe(today);
	}

	[Fact]
	public async Task Handle_WhenAdoptionDateIsSetAndUserIsOwner_ShouldClearAdoptionDateAndReturnResponse()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var existingDate = new DateOnly(2024, 1, 15);
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: existingDate);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		adoptionAlert.AdoptionDate.ShouldBeNull();
		await _adoptionAlertRepository.Received(1).CommitAsync();
		_valueProvider.DidNotReceive().DateOnlyNow();

		result.ShouldNotBeNull();
		result.Id.ShouldBe(alertId);
		result.AdoptionDate.ShouldBeNull();
	}

	[Fact]
	public async Task Handle_WhenAlertDoesNotExist_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ToggleAdoptionCommand(alertId, userId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns((AdoptionAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(async () =>
				await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Alerta de adoção com o id especificado não existe.");

		await _adoptionAlertRepository.DidNotReceive().CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenAlertDoesNotExist_ShouldLogInformation()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ToggleAdoptionCommand(alertId, userId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns((AdoptionAlert?)null);

		// Act & Assert
		await Should.ThrowAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));

		_logger.Received(1).Log(
			LogLevel.Information,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o.ToString()!.Contains(alertId.ToString())),
			null,
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldThrowForbiddenException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var differentUserId = Guid.NewGuid();
		var command = new ToggleAdoptionCommand(alertId, differentUserId);

		var adoptionAlert = CreateAdoptionAlert(alertId, ownerId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(async () =>
				await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Não é possível alterar o status de alertas em que não é dono.");

		await _adoptionAlertRepository.DidNotReceive().CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldLogInformation()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var differentUserId = Guid.NewGuid();
		var command = new ToggleAdoptionCommand(alertId, differentUserId);

		var adoptionAlert = CreateAdoptionAlert(alertId, ownerId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act & Assert
		await Should.ThrowAsync<ForbiddenException>(async () => await _handler.Handle(command, CancellationToken.None));

		_logger.Received(1).Log(
			LogLevel.Information,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o.ToString()!.Contains(differentUserId.ToString())
			                    && o.ToString()!.Contains(ownerId.ToString())),
			null,
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldNotModifyAdoptionDate()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var differentUserId = Guid.NewGuid();
		var originalDate = new DateOnly(2024, 1, 10);
		var command = new ToggleAdoptionCommand(alertId, differentUserId);

		var adoptionAlert = CreateAdoptionAlert(alertId, ownerId, adoptionDate: originalDate);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act & Assert
		await Should.ThrowAsync<ForbiddenException>(async () => await _handler.Handle(command, CancellationToken.None));

		adoptionAlert.AdoptionDate.ShouldBe(originalDate);
		_valueProvider.DidNotReceive().DateOnlyNow();
	}

	[Fact]
	public async Task Handle_WhenSuccessful_ShouldReturnAdoptionAlertResponseWithCorrectData()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var today = DateOnly.FromDateTime(DateTime.Today);
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_valueProvider.DateOnlyNow().Returns(today);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldNotBeNull();
		result.Id.ShouldBe(alertId);
		result.Neighborhood.ShouldBe(adoptionAlert.Neighborhood);
		result.AdoptionDate.ShouldBe(today);
		result.Pet.ShouldNotBeNull();
		result.Pet.Name.ShouldBe(adoptionAlert.Pet.Name);
		result.Owner.ShouldNotBeNull();
		result.Owner.Id.ShouldBe(userId);
	}

	[Fact]
	public async Task Handle_WhenToggleFromNullToDate_ShouldUseCorrectDateFromProvider()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var specificDate = new DateOnly(2024, 6, 15);
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_valueProvider.DateOnlyNow().Returns(specificDate);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		adoptionAlert.AdoptionDate.ShouldBe(specificDate);
		result.AdoptionDate.ShouldBe(specificDate);
	}

	[Fact]
	public async Task Handle_WhenToggleMultipleTimes_ShouldToggleBetweenNullAndDate()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var today = DateOnly.FromDateTime(DateTime.Today);
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_valueProvider.DateOnlyNow().Returns(today);

		// Act
		var firstResult = await _handler.Handle(command, CancellationToken.None);
		var firstToggleDate = adoptionAlert.AdoptionDate;

		// Act
		var secondResult = await _handler.Handle(command, CancellationToken.None);
		var secondToggleDate = adoptionAlert.AdoptionDate;

		// Assert
		firstToggleDate.ShouldBe(today);
		firstResult.AdoptionDate.ShouldBe(today);

		secondToggleDate.ShouldBeNull();
		secondResult.AdoptionDate.ShouldBeNull();

		await _adoptionAlertRepository.Received(2).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenCancellationRequested_ShouldRespectCancellationToken()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ToggleAdoptionCommand(alertId, userId);
		var cancellationToken = new CancellationToken(canceled: true);

		_adoptionAlertRepository.GetByIdAsync(alertId)
			.Throws(new OperationCanceledException());

		// Act & Assert
		await Should.ThrowAsync<OperationCanceledException>(async () =>
			await _handler.Handle(command, cancellationToken));
	}

	[Fact]
	public async Task Handle_WhenCommitFails_ShouldPropagateException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);
		_valueProvider.DateOnlyNow().Returns(DateOnly.FromDateTime(DateTime.Today));
		_adoptionAlertRepository.CommitAsync().Throws(new Exception("Database error"));

		// Act & Assert
		var exception =
			await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Database error");
	}

	[Theory]
	[InlineData(2024, 1, 1)]
	[InlineData(2024, 6, 15)]
	[InlineData(2024, 12, 31)]
	public async Task Handle_WithDifferentDates_ShouldClearToNullAndReturnCorrectResponse(int year, int month, int day)
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var existingDate = new DateOnly(year, month, day);
		var command = new ToggleAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId, adoptionDate: existingDate);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		adoptionAlert.AdoptionDate.ShouldBeNull();
		result.AdoptionDate.ShouldBeNull();
	}

	private static AdoptionAlert CreateAdoptionAlert(Guid alertId, Guid userId, DateOnly? adoptionDate)
	{
		var user = new User
		{
			Id = userId,
			Image = "user.jpg",
			FullName = "Test User",
			Email = "test@example.com",
			PhoneNumber = "+1234567890",
			DefaultAdoptionFormUrl = null
		};

		var pet = new Pet
		{
			Id = Guid.NewGuid(),
			Name = "Test Pet",
			Gender = Gender.Fêmea,
			Age = Age.Adulto,
			Size = Size.Médio,
			Images = [],
			Colors = new List<Color> { new Color { Id = 1, Name = "Brown" } },
			IsVaccinated = true,
			IsCastrated = false,
			IsNegativeToFivFelv = null,
			IsNegativeToLeishmaniasis = null,
			Breed = new Breed { Id = 1, Name = "Mixed" },
			Species = new Species { Id = 1, Name = "Dog" }
		};

		return new AdoptionAlert
		{
			Id = alertId,
			User = user,
			Pet = pet,
			AdoptionDate = adoptionDate,
			AdoptionRestrictions = new List<string> { "No restrictions" },
			Neighborhood = "Test Neighborhood",
			Description = "Test Description",
			AdoptionForm = null,
			RegistrationDate = DateTime.UtcNow
		};
	}
}
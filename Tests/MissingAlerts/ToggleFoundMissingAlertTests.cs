using Application.Commands.MissingAlerts.ToggleFound;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Tests.MissingAlerts;

public sealed class ToggleFoundMissingAlertCommandHandlerTests
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IValueProvider _valueProvider;
	private readonly ToggleFoundMissingAlertCommandHandler _handler;

	public ToggleFoundMissingAlertCommandHandlerTests()
	{
		_missingAlertRepository = Substitute.For<IMissingAlertRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		var logger = Substitute.For<ILogger<ToggleFoundMissingAlertCommandHandler>>();

		_handler = new ToggleFoundMissingAlertCommandHandler(
			_missingAlertRepository,
			_valueProvider,
			logger);
	}

	[Fact]
	public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new ToggleFoundMissingAlertCommand(Guid.NewGuid(), Guid.NewGuid());
		_missingAlertRepository.GetByIdAsync(command.AlertId).Returns((MissingAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alerta com o id especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldThrowForbiddenException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var command = new ToggleFoundMissingAlertCommand(alertId, userId);

		var missingAlert = CreateMissingAlert(alertId, ownerId);
		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível marcar alertas de outros usuários como encontrado.");
	}

	[Fact]
	public async Task Handle_WhenRecoveryDateIsNull_ShouldSetRecoveryDate()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ToggleFoundMissingAlertCommand(alertId, userId);
		var today = DateOnly.FromDateTime(DateTime.UtcNow);

		var missingAlert = CreateMissingAlert(alertId, userId, recoveryDate: null);
		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);
		_valueProvider.DateOnlyNow().Returns(today);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		missingAlert.RecoveryDate.ShouldBe(today);
		await _missingAlertRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenRecoveryDateIsSet_ShouldClearRecoveryDate()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ToggleFoundMissingAlertCommand(alertId, userId);
		var existingDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));

		var missingAlert = CreateMissingAlert(alertId, userId, recoveryDate: existingDate);
		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		missingAlert.RecoveryDate.ShouldBeNull();
		await _missingAlertRepository.Received(1).CommitAsync();
	}

	private static MissingAlert CreateMissingAlert(Guid alertId, Guid ownerId, DateOnly? recoveryDate = null)
	{
		return new MissingAlert
		{
			Id = alertId,
			User = new User { Id = ownerId },
			Location = new Point(0, 0),
			RecoveryDate = recoveryDate
		};
	}
}
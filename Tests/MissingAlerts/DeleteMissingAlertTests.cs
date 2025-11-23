using Application.Commands.MissingAlerts.Delete;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Domain.Entities;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Tests.MissingAlerts;

public sealed class DeleteMissingAlertCommandHandlerTests
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly DeleteMissingAlertCommandHandler _handler;

	public DeleteMissingAlertCommandHandlerTests()
	{
		_missingAlertRepository = Substitute.For<IMissingAlertRepository>();
		var logger = Substitute.For<ILogger<DeleteMissingAlertCommandHandler>>();

		_handler = new DeleteMissingAlertCommandHandler(
			_missingAlertRepository,
			logger);
	}

	[Fact]
	public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new DeleteMissingAlertCommand(Guid.NewGuid(), Guid.NewGuid());
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
		var command = new DeleteMissingAlertCommand(alertId, userId);

		var missingAlert = new MissingAlert
		{
			Id = alertId,
			User = new User
			{
				Id = ownerId
			},
			Location = new Point(0, 0)
		};

		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<ForbiddenException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Não é possível excluir alertas de outros usuários.");
	}

	[Fact]
	public async Task Handle_WhenUserIsOwner_ShouldDeleteAlert()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new DeleteMissingAlertCommand(alertId, userId);

		var missingAlert = new MissingAlert
		{
			Id = alertId,
			User = new User
			{
				Id = userId
			},
			Location = new Point(0, 0)
		};

		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		_missingAlertRepository.Received(1).Delete(missingAlert);
		await _missingAlertRepository.Received(1).CommitAsync();
	}
}
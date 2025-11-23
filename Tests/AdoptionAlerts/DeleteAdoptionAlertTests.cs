using Application.Commands.AdoptionAlerts.Delete;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Domain.Entities;
using Domain.Entities.Alerts;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace Tests.AdoptionAlerts;

public sealed class DeleteAdoptionCommandHandlerTests
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly ILogger<DeleteAdoptionCommandHandler> _logger;
	private readonly DeleteAdoptionCommandHandler _handler;

	public DeleteAdoptionCommandHandlerTests()
	{
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_logger = Substitute.For<ILogger<DeleteAdoptionCommandHandler>>();
		_handler = new DeleteAdoptionCommandHandler(_adoptionAlertRepository, _logger);
	}

	[Fact]
	public async Task Handle_WhenAlertExistsAndUserIsOwner_ShouldDeleteAlertAndCommit()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new DeleteAdoptionCommand(alertId, userId);

		var adoptionAlert = CreateAdoptionAlert(alertId, userId);
		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionAlertRepository.Received(1).Delete(adoptionAlert);
		await _adoptionAlertRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenAlertDoesNotExist_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new DeleteAdoptionCommand(alertId, userId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns((AdoptionAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(async () =>
				await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Alerta de adoção com o id especificado não existe.");

		_adoptionAlertRepository.DidNotReceive().Delete(Arg.Any<AdoptionAlert>());
		await _adoptionAlertRepository.DidNotReceive().CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenAlertDoesNotExist_ShouldLogInformation()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new DeleteAdoptionCommand(alertId, userId);

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
	public async Task Handle_WhenUserIsNotOwner_ShouldThrowUnauthorizedException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var differentUserId = Guid.NewGuid();
		var command = new DeleteAdoptionCommand(alertId, differentUserId);

		var adoptionAlert = CreateAdoptionAlert(alertId, ownerId);
		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<UnauthorizedException>(async () =>
				await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Não é possível alterar alertas de adoção de outros usuários.");

		_adoptionAlertRepository.DidNotReceive().Delete(Arg.Any<AdoptionAlert>());
		await _adoptionAlertRepository.DidNotReceive().CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenUserIsNotOwner_ShouldLogInformation()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var ownerId = Guid.NewGuid();
		var differentUserId = Guid.NewGuid();
		var command = new DeleteAdoptionCommand(alertId, differentUserId);

		var adoptionAlert = CreateAdoptionAlert(alertId, ownerId);
		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(adoptionAlert);

		// Act & Assert
		await Should.ThrowAsync<UnauthorizedException>(async () =>
			await _handler.Handle(command, CancellationToken.None));

		_logger.Received(1).Log(
			LogLevel.Information,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o.ToString()!.Contains(differentUserId.ToString())
			                    && o.ToString()!.Contains(ownerId.ToString())),
			null,
			Arg.Any<Func<object, Exception?, string>>());
	}

	[Fact]
	public async Task Handle_WhenCancellationRequested_ShouldRespectCancellationToken()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new DeleteAdoptionCommand(alertId, userId);
		var cancellationToken = new CancellationToken(canceled: true);

		_adoptionAlertRepository.GetByIdAsync(alertId)
			.Throws(new OperationCanceledException());

		// Act & Assert
		await Should.ThrowAsync<OperationCanceledException>(async () =>
			await _handler.Handle(command, cancellationToken));
	}

	private static AdoptionAlert CreateAdoptionAlert(Guid alertId, Guid userId)
	{
		var user = new User { Id = userId };
		return new AdoptionAlert
		{
			Id = alertId,
			User = user,
			Neighborhood = "neighborhood"
		};
	}
}
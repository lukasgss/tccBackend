using Application.Commands.MissingAlerts.ReportMissingAlert;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AdoptionReports;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Tests.MissingAlerts;

public sealed class ReportMissingAlertCommandHandlerTests
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IMissingReportRepository _missingReportRepository;
	private readonly IValueProvider _valueProvider;
	private readonly IUserRepository _userRepository;
	private readonly ReportMissingAlertCommandHandler _handler;

	public ReportMissingAlertCommandHandlerTests()
	{
		_missingAlertRepository = Substitute.For<IMissingAlertRepository>();
		_missingReportRepository = Substitute.For<IMissingReportRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		_userRepository = Substitute.For<IUserRepository>();
		var logger = Substitute.For<ILogger<ReportMissingAlertCommandHandler>>();

		_handler = new ReportMissingAlertCommandHandler(
			_valueProvider,
			Substitute.For<IAdoptionReportRepository>(),
			_userRepository,
			_missingAlertRepository,
			logger,
			_missingReportRepository);
	}

	[Fact]
	public async Task Handle_WhenAlertNotFound_ShouldThrowNotFoundException()
	{
		// Arrange
		var command = new ReportMissingAlertCommand(Guid.NewGuid(), "Test Reason", Guid.NewGuid());
		_missingAlertRepository.GetByIdAsync(command.AlertId).Returns((MissingAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		exception.Message.ShouldBe("Alerta especificado não existe.");
	}

	[Fact]
	public async Task Handle_WhenUserIdIsNull_ShouldCreateReportWithoutUser()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var reportId = Guid.NewGuid();
		var now = DateTime.UtcNow;
		var command = new ReportMissingAlertCommand(alertId, "Test Reason", null);

		var missingAlert = CreateMissingAlert(alertId);
		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);
		_valueProvider.NewGuid().Returns(reportId);
		_valueProvider.UtcNow().Returns(now);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		_missingReportRepository.Received(1).Add(Arg.Is<MissingAnimalReport>(r =>
			r.Id == reportId &&
			r.Reason == "Test Reason" &&
			r.Owner == null &&
			r.Status == ReportStatus.Sent));
		await _missingReportRepository.Received(1).CommitAsync();
		await _userRepository.DidNotReceive().GetUserByIdAsync(Arg.Any<Guid>());
	}

	[Fact]
	public async Task Handle_WhenUserIdProvided_ShouldCreateReportWithUser()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var reportId = Guid.NewGuid();
		var now = DateTime.UtcNow;
		var command = new ReportMissingAlertCommand(alertId, "Test Reason", userId);

		var missingAlert = CreateMissingAlert(alertId);
		var user = new User { Id = userId };

		_missingAlertRepository.GetByIdAsync(alertId).Returns(missingAlert);
		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_valueProvider.NewGuid().Returns(reportId);
		_valueProvider.UtcNow().Returns(now);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);
		_missingReportRepository.Received(1).Add(Arg.Is<MissingAnimalReport>(r =>
			r.Id == reportId &&
			r.Reason == "Test Reason" &&
			r.Owner == user &&
			r.Status == ReportStatus.Sent));
		await _missingReportRepository.Received(1).CommitAsync();
	}

	private static MissingAlert CreateMissingAlert(Guid alertId)
	{
		return new MissingAlert
		{
			Id = alertId,
			Location = new NetTopologySuite.Geometries.Point(0, 0),
			User = new User { Id = Guid.NewGuid() }
		};
	}
}
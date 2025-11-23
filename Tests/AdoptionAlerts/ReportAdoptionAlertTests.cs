using Application.Commands.AdoptionAlerts.ReportAdoptionAlert;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AdoptionReports;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using MediatR;
using NSubstitute.ExceptionExtensions;

namespace Tests.AdoptionAlerts;

public sealed class ReportAdoptionAlertTests
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IValueProvider _valueProvider;
	private readonly IAdoptionReportRepository _adoptionReportRepository;
	private readonly IUserRepository _userRepository;
	private readonly ReportAdoptionAlertCommandHandler _handler;

	public ReportAdoptionAlertTests()
	{
		_adoptionAlertRepository = Substitute.For<IAdoptionAlertRepository>();
		_valueProvider = Substitute.For<IValueProvider>();
		_adoptionReportRepository = Substitute.For<IAdoptionReportRepository>();
		_userRepository = Substitute.For<IUserRepository>();

		_handler = new ReportAdoptionAlertCommandHandler(
			_adoptionAlertRepository,
			_valueProvider,
			_adoptionReportRepository,
			_userRepository);
	}

	[Fact]
	public async Task Handle_WhenAlertExistsAndUserIdProvided_ShouldCreateReportWithUser()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var reportId = Guid.NewGuid();
		var now = DateTime.UtcNow;
		var reason = "Inappropriate content";
		var command = new ReportAdoptionAlertCommand(alertId, reason, userId);

		var alert = CreateAdoptionAlert(alertId);
		var user = new User { Id = userId };

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_userRepository.GetUserByIdAsync(userId).Returns(user);
		_valueProvider.NewGuid().Returns(reportId);
		_valueProvider.UtcNow().Returns(now);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);

		_adoptionReportRepository.Received(1).Add(Arg.Is<AdoptionReport>(r =>
			r.Id == reportId &&
			r.Reason == reason &&
			r.Status == ReportStatus.Sent &&
			r.CreatedAt == now &&
			r.RejectedReason == null &&
			r.ResolvedAt == null &&
			r.Owner == user));

		await _adoptionReportRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenAlertExistsAndUserIdIsNull_ShouldCreateReportWithoutUser()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var reportId = Guid.NewGuid();
		var now = DateTime.UtcNow;
		var reason = "Spam content";
		var command = new ReportAdoptionAlertCommand(alertId, reason, null);

		var alert = CreateAdoptionAlert(alertId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_valueProvider.NewGuid().Returns(reportId);
		_valueProvider.UtcNow().Returns(now);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);

		await _userRepository.DidNotReceive().GetUserByIdAsync(Arg.Any<Guid>());

		_adoptionReportRepository.Received(1).Add(Arg.Is<AdoptionReport>(r =>
			r.Id == reportId &&
			r.Reason == reason &&
			r.Status == ReportStatus.Sent &&
			r.CreatedAt == now &&
			r.RejectedReason == null &&
			r.ResolvedAt == null &&
			r.Owner == null));

		await _adoptionReportRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenAlertDoesNotExist_ShouldThrowNotFoundException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var command = new ReportAdoptionAlertCommand(alertId, "Test reason", null);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns((AdoptionAlert?)null);

		// Act & Assert
		var exception =
			await Should.ThrowAsync<NotFoundException>(async () =>
				await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Alerta especificado não existe.");

		_adoptionReportRepository.DidNotReceive().Add(Arg.Any<AdoptionReport>());
		await _adoptionReportRepository.DidNotReceive().CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenAlertDoesNotExist_ShouldNotFetchUser()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var command = new ReportAdoptionAlertCommand(alertId, "Test reason", userId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns((AdoptionAlert?)null);

		// Act & Assert
		await Should.ThrowAsync<NotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));

		await _userRepository.DidNotReceive().GetUserByIdAsync(Arg.Any<Guid>());
	}

	[Fact]
	public async Task Handle_WhenUserIdProvidedButUserNotFound_ShouldCreateReportWithNullOwner()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		var reportId = Guid.NewGuid();
		var now = DateTime.UtcNow;
		var reason = "Test reason";
		var command = new ReportAdoptionAlertCommand(alertId, reason, userId);

		var alert = CreateAdoptionAlert(alertId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_userRepository.GetUserByIdAsync(userId).Returns((User?)null);
		_valueProvider.NewGuid().Returns(reportId);
		_valueProvider.UtcNow().Returns(now);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.ShouldBe(Unit.Value);

		await _userRepository.Received(1).GetUserByIdAsync(userId);

		_adoptionReportRepository.Received(1).Add(Arg.Is<AdoptionReport>(r =>
			r.Owner == null));

		await _adoptionReportRepository.Received(1).CommitAsync();
	}

	[Fact]
	public async Task Handle_WhenCalled_ShouldUseValueProviderForIdAndTimestamp()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var reportId = Guid.NewGuid();
		var now = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
		var command = new ReportAdoptionAlertCommand(alertId, "Test", null);

		var alert = CreateAdoptionAlert(alertId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_valueProvider.NewGuid().Returns(reportId);
		_valueProvider.UtcNow().Returns(now);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_valueProvider.Received(1).NewGuid();
		_valueProvider.Received(1).UtcNow();
	}

	[Fact]
	public async Task Handle_WhenCommitFails_ShouldPropagateException()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var command = new ReportAdoptionAlertCommand(alertId, "Test", null);
		var alert = CreateAdoptionAlert(alertId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_valueProvider.NewGuid().Returns(Guid.NewGuid());
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);
		_adoptionReportRepository.CommitAsync().Throws(new Exception("Database error"));

		// Act & Assert
		var exception =
			await Should.ThrowAsync<Exception>(async () => await _handler.Handle(command, CancellationToken.None));

		exception.Message.ShouldBe("Database error");
	}

	[Fact]
	public async Task Handle_WhenCancellationRequested_ShouldRespectCancellationToken()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var command = new ReportAdoptionAlertCommand(alertId, "Test", null);
		var cancellationToken = new CancellationToken(canceled: true);

		_adoptionAlertRepository.GetByIdAsync(alertId)
			.Throws(new OperationCanceledException());

		// Act & Assert
		await Should.ThrowAsync<OperationCanceledException>(async () =>
			await _handler.Handle(command, cancellationToken));
	}

	[Theory]
	[InlineData("Spam content")]
	[InlineData("Inappropriate images")]
	[InlineData("Scam alert")]
	[InlineData("Abusive language")]
	public async Task Handle_WithDifferentReasons_ShouldCreateReportWithCorrectReason(string reason)
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var command = new ReportAdoptionAlertCommand(alertId, reason, null);
		var alert = CreateAdoptionAlert(alertId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_valueProvider.NewGuid().Returns(Guid.NewGuid());
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionReportRepository.Received(1).Add(Arg.Is<AdoptionReport>(r =>
			r.Reason == reason));
	}

	[Fact]
	public async Task Handle_WhenCalled_ShouldSetReportStatusToSent()
	{
		// Arrange
		var alertId = Guid.NewGuid();
		var command = new ReportAdoptionAlertCommand(alertId, "Test", null);
		var alert = CreateAdoptionAlert(alertId);

		_adoptionAlertRepository.GetByIdAsync(alertId).Returns(alert);
		_valueProvider.NewGuid().Returns(Guid.NewGuid());
		_valueProvider.UtcNow().Returns(DateTime.UtcNow);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		_adoptionReportRepository.Received(1).Add(Arg.Is<AdoptionReport>(r =>
			r.Status == ReportStatus.Sent));
	}

	private static AdoptionAlert CreateAdoptionAlert(Guid alertId)
	{
		return new AdoptionAlert
		{
			Id = alertId,
			Neighborhood = "neighborhood"
		};
	}
}
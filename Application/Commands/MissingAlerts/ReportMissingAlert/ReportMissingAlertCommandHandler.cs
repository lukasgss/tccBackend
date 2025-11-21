using Application.Common.Interfaces.Entities.AdoptionReports;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.MissingAlerts.ReportMissingAlert;

public sealed record ReportMissingAlertCommand(Guid AlertId, string Reason, Guid? UserId) : IRequest<Unit>;

public sealed class ReportMissingAlertCommandHandler : IRequestHandler<ReportMissingAlertCommand, Unit>
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IMissingReportRepository _missingReportRepository;
	private readonly IValueProvider _valueProvider;
	private readonly IUserRepository _userRepository;
	private readonly ILogger<ReportMissingAlertCommandHandler> _logger;

	public ReportMissingAlertCommandHandler(
		IValueProvider valueProvider,
		IAdoptionReportRepository adoptionReportRepository,
		IUserRepository userRepository,
		IMissingAlertRepository missingAlertRepository,
		ILogger<ReportMissingAlertCommandHandler> logger,
		IMissingReportRepository missingReportRepository)
	{
		_missingAlertRepository = missingAlertRepository;
		_missingReportRepository = missingReportRepository;
		_userRepository = Guard.Against.Null(userRepository);
		_valueProvider = Guard.Against.Null(valueProvider);
		_logger = logger;
	}

	public async Task<Unit> Handle(ReportMissingAlertCommand request, CancellationToken cancellationToken)
	{
		MissingAlert? missingAlert = await _missingAlertRepository.GetByIdAsync(request.AlertId);
		if (missingAlert is null)
		{
			_logger.LogInformation("Tentativa de denunciar alerta {AlertId}, mas alerta não existe", request.AlertId);
			throw new NotFoundException("Alerta especificado não existe.");
		}

		User? user = null;
		if (request.UserId is not null)
		{
			user = await _userRepository.GetUserByIdAsync(request.UserId.Value);
		}

		MissingAnimalReport animalReportMissingAnimalAlert = new()
		{
			Id = _valueProvider.NewGuid(),
			CreatedAt = _valueProvider.UtcNow(),
			Reason = request.Reason,
			Status = ReportStatus.Sent,
			RejectedReason = null,
			ResolvedAt = null,
			Owner = user
		};

		_missingReportRepository.Add(animalReportMissingAnimalAlert);
		await _missingReportRepository.CommitAsync();

		return Unit.Value;
	}
}
using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.AdoptionReports;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Enums;
using MediatR;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionAlerts.ReportAdoptionAlert;

[ExcludeFromCodeCoverage]
public sealed record ReportAdoptionAlertCommand(Guid AlertId, string Reason, Guid? UserId) : IRequest<Unit>;

public sealed class ReportAdoptionAlertCommandHandler : IRequestHandler<ReportAdoptionAlertCommand, Unit>
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly IValueProvider _valueProvider;
    private readonly IAdoptionReportRepository _adoptionReportRepository;
    private readonly IUserRepository _userRepository;

    public ReportAdoptionAlertCommandHandler(
        IAdoptionAlertRepository adoptionAlertRepository,
        IValueProvider valueProvider,
        IAdoptionReportRepository adoptionReportRepository, IUserRepository userRepository)
    {
        _userRepository = Guard.Against.Null(userRepository);
        _adoptionReportRepository = Guard.Against.Null(adoptionReportRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
    }

    public async Task<Unit> Handle(ReportAdoptionAlertCommand request, CancellationToken cancellationToken)
    {
        AdoptionAlert? reportedAlert = await _adoptionAlertRepository.GetByIdAsync(request.AlertId);
        if (reportedAlert is null)
        {
            throw new NotFoundException("Alerta especificado não existe.");
        }

        User? user = null;
        if (request.UserId is not null)
        {
            user = await _userRepository.GetUserByIdAsync(request.UserId.Value);
        }

        AdoptionReport reportAdoptionAlert = new()
        {
            Id = _valueProvider.NewGuid(),
            CreatedAt = _valueProvider.UtcNow(),
            Reason = request.Reason,
            Status = ReportStatus.Sent,
            RejectedReason = null,
            ResolvedAt = null,
            Owner = user
        };

        _adoptionReportRepository.Add(reportAdoptionAlert);
        await _adoptionReportRepository.CommitAsync();

        return Unit.Value;
    }
}
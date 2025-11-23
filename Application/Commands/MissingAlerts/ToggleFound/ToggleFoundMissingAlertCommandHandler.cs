using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.MissingAlerts.ToggleFound;

[ExcludeFromCodeCoverage]
public sealed record ToggleFoundMissingAlertCommand(Guid AlertId, Guid UserId) : IRequest<Unit>;

public class ToggleFoundMissingAlertCommandHandler : IRequestHandler<ToggleFoundMissingAlertCommand, Unit>
{
    private readonly IMissingAlertRepository _missingAlertRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ILogger<ToggleFoundMissingAlertCommandHandler> _logger;

    public ToggleFoundMissingAlertCommandHandler(
        IMissingAlertRepository missingAlertRepository,
        IValueProvider valueProvider,
        ILogger<ToggleFoundMissingAlertCommandHandler> logger)
    {
        _missingAlertRepository = Guard.Against.Null(missingAlertRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<Unit> Handle(ToggleFoundMissingAlertCommand request, CancellationToken cancellationToken)
    {
        MissingAlert missingAlert = await ValidateAndAssignMissingAlertAsync(request.AlertId);

        if (request.UserId != missingAlert.User.Id)
        {
            _logger.LogInformation(
                "Usuário {UserId} não possui permissão para alterar status do alerta {MissingAlertId}",
                request.UserId, request.AlertId);
            throw new ForbiddenException("Não é possível marcar alertas de outros usuários como encontrado.");
        }

        if (missingAlert.RecoveryDate is null)
        {
            missingAlert.RecoveryDate = _valueProvider.DateOnlyNow();
        }
        else
        {
            missingAlert.RecoveryDate = null;
        }

        await _missingAlertRepository.CommitAsync();

        return Unit.Value;
    }

    private async Task<MissingAlert> ValidateAndAssignMissingAlertAsync(Guid missingAlertId)
    {
        MissingAlert? dbMissingAlert = await _missingAlertRepository.GetByIdAsync(missingAlertId);
        if (dbMissingAlert is null)
        {
            _logger.LogInformation("Alerta {MissingAlertId} não existe", missingAlertId);
            throw new NotFoundException("Alerta com o id especificado não existe.");
        }

        return dbMissingAlert;
    }
}
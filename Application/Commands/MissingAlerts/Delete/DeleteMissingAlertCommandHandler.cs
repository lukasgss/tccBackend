using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.MissingAlerts.Delete;

[ExcludeFromCodeCoverage]
public sealed record DeleteMissingAlertCommand(Guid AlertId, Guid UserId) : IRequest<Unit>;

public class DeleteMissingAlertCommandHandler : IRequestHandler<DeleteMissingAlertCommand, Unit>
{
    private readonly IMissingAlertRepository _missingAlertRepository;
    private readonly ILogger<DeleteMissingAlertCommandHandler> _logger;

    public DeleteMissingAlertCommandHandler(
        IMissingAlertRepository missingAlertRepository,
        ILogger<DeleteMissingAlertCommandHandler> logger)
    {
        _missingAlertRepository = Guard.Against.Null(missingAlertRepository);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<Unit> Handle(DeleteMissingAlertCommand request, CancellationToken cancellationToken)
    {
        MissingAlert alertToDelete = await ValidateAndAssignMissingAlertAsync(request.AlertId);

        if (alertToDelete.User.Id != request.UserId)
        {
            _logger.LogInformation(
                "Usuário {UserId} não possui permissão para excluir alerta {MissingAlertId}",
                request.UserId,
                request.AlertId);
            throw new ForbiddenException("Não é possível excluir alertas de outros usuários.");
        }

        _missingAlertRepository.Delete(alertToDelete);
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
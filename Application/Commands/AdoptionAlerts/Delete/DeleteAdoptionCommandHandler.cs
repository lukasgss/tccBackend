using System.Diagnostics.CodeAnalysis;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionAlerts.Delete;

[ExcludeFromCodeCoverage]
public sealed record DeleteAdoptionCommand(Guid AlertId, Guid UserId) : IRequest;

public sealed class DeleteAdoptionCommandHandler : IRequestHandler<DeleteAdoptionCommand>
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly ILogger<DeleteAdoptionCommandHandler> _logger;

    public DeleteAdoptionCommandHandler(
        IAdoptionAlertRepository adoptionAlertRepository,
        ILogger<DeleteAdoptionCommandHandler> logger)
    {
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
        _logger = Guard.Against.Null(logger);
    }

    public async Task Handle(DeleteAdoptionCommand request, CancellationToken cancellationToken)
    {
        var adoptionAlertDb = await ValidateAndAssignAdoptionAlertAsync(request.AlertId);
        ValidateIfUserIsOwnerOfAlert(adoptionAlertDb.User.Id, request.UserId);

        _adoptionAlertRepository.Delete(adoptionAlertDb);
        await _adoptionAlertRepository.CommitAsync();
    }

    private async Task<AdoptionAlert> ValidateAndAssignAdoptionAlertAsync(Guid alertId)
    {
        var adoptionAlert = await _adoptionAlertRepository.GetByIdAsync(alertId);
        if (adoptionAlert is null)
        {
            _logger.LogInformation("Alerta de adoção {AlertId} não existe", alertId);
            throw new NotFoundException("Alerta de adoção com o id especificado não existe.");
        }

        return adoptionAlert;
    }

    private void ValidateIfUserIsOwnerOfAlert(Guid actualOwnerId, Guid userId)
    {
        if (actualOwnerId != userId)
        {
            _logger.LogInformation(
                "Usuário {UserId} não possui permissão para alterar adoção em que o dono é {ActualAlertOwnerId}",
                userId, actualOwnerId);
            throw new UnauthorizedException("Não é possível alterar alertas de adoção de outros usuários.");
        }
    }
}
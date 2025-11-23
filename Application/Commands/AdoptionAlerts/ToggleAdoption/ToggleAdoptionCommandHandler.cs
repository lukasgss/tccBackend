using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionAlerts.ToggleAdoption;

[ExcludeFromCodeCoverage]
public sealed record ToggleAdoptionCommand(Guid AlertId, Guid UserId) : IRequest<AdoptionAlertResponse>;

public sealed class ToggleAdoptionAlertCommandHandler : IRequestHandler<ToggleAdoptionCommand, AdoptionAlertResponse>
{
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ILogger<ToggleAdoptionAlertCommandHandler> _logger;

    public ToggleAdoptionAlertCommandHandler(
        IAdoptionAlertRepository adoptionAlertRepository,
        IValueProvider valueProvider,
        ILogger<ToggleAdoptionAlertCommandHandler> logger)
    {
        _valueProvider = Guard.Against.Null(valueProvider);
        _logger = Guard.Against.Null(logger);
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
    }

    public async Task<AdoptionAlertResponse> Handle(ToggleAdoptionCommand request, CancellationToken cancellationToken)
    {
        var adoptionAlert = await ValidateAndAssignAdoptionAlertAsync(request.AlertId);

        if (request.UserId != adoptionAlert.User.Id)
        {
            _logger.LogInformation(
                "Usuário {UserId} não possui permissão para alterar status de adoção em que o dono é {ActualAlertOwnerId}",
                request.UserId, adoptionAlert.User.Id);
            throw new ForbiddenException("Não é possível alterar o status de alertas em que não é dono.");
        }

        if (adoptionAlert.AdoptionDate is null)
        {
            adoptionAlert.AdoptionDate = _valueProvider.DateOnlyNow();
        }
        else
        {
            adoptionAlert.AdoptionDate = null;
        }

        await _adoptionAlertRepository.CommitAsync();

        return adoptionAlert.ToAdoptionAlertResponse();
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
}
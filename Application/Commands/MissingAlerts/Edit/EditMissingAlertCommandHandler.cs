using System.Diagnostics.CodeAnalysis;
using Application.Common.Calculators;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.MissingAlerts.Edit;

[ExcludeFromCodeCoverage]
public sealed record EditMissingAlertCommand(
    Guid Id,
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    Guid PetId,
    Guid UserId
) : IRequest<MissingAlertResponse>;

public sealed class EditMissingAlertCommandHandler : IRequestHandler<EditMissingAlertCommand, MissingAlertResponse>
{
    private readonly IMissingAlertRepository _missingAlertRepository;
    private readonly IPetRepository _petRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EditMissingAlertCommandHandler> _logger;

    public EditMissingAlertCommandHandler(
        IMissingAlertRepository missingAlertRepository,
        IPetRepository petRepository,
        IUserRepository userRepository,
        ILogger<EditMissingAlertCommandHandler> logger)
    {
        _missingAlertRepository = Guard.Against.Null(missingAlertRepository);
        _petRepository = Guard.Against.Null(petRepository);
        _userRepository = Guard.Against.Null(userRepository);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<MissingAlertResponse> Handle(EditMissingAlertCommand request, CancellationToken cancellationToken)
    {
        MissingAlert dbMissingAlert = await ValidateAndAssignMissingAlertAsync(request.Id);
        Pet pet = await ValidateAndAssignPetAsync(request.PetId);

        CheckUserPermissionToEdit(dbMissingAlert.User.Id, request.UserId);

        User user = await ValidateAndAssignUserAsync(request.UserId);

        Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
            request.LastSeenLocationLatitude,
            request.LastSeenLocationLongitude);

        dbMissingAlert.Location = location;
        dbMissingAlert.Description = request.Description;
        dbMissingAlert.Pet = pet;
        dbMissingAlert.User = user;

        await _missingAlertRepository.CommitAsync();

        return dbMissingAlert.ToMissingAlertResponse();
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

    private async Task<Pet> ValidateAndAssignPetAsync(Guid petId)
    {
        Pet? pet = await _petRepository.GetPetByIdAsync(petId);
        if (pet is null)
        {
            _logger.LogInformation("Pet {PetId} não existe", petId);
            throw new NotFoundException("Animal com o id especificado não existe.");
        }

        return pet;
    }

    private async Task<User> ValidateAndAssignUserAsync(Guid userId)
    {
        User? user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            _logger.LogInformation("Usuário {UserId} não existe", userId);
            throw new NotFoundException("Usuário com o id especificado não existe.");
        }

        return user;
    }

    private void CheckUserPermissionToEdit(Guid? userId, Guid requestUserId)
    {
        if (userId != requestUserId)
        {
            _logger.LogInformation(
                "Usuário {UserId} não possui permissão para editar alerta do usuário {ActualOwnerId}",
                userId, requestUserId);
            throw new ForbiddenException("Não é possível editar alertas de outros usuários.");
        }
    }
}
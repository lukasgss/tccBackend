using Application.Common.Calculators;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.MissingAlerts.Create;

public record CreateMissingAlertCommand(
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    Guid PetId,
    Guid UserId
) : IRequest<MissingAlertResponse>;

public class CreateMissingAlertCommandHandler : IRequestHandler<CreateMissingAlertCommand, MissingAlertResponse>
{
    private readonly IMissingAlertRepository _missingAlertRepository;
    private readonly IPetRepository _petRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ILogger<CreateMissingAlertCommandHandler> _logger;

    public CreateMissingAlertCommandHandler(
        IMissingAlertRepository missingAlertRepository,
        IPetRepository petRepository,
        IUserRepository userRepository,
        IValueProvider valueProvider,
        ILogger<CreateMissingAlertCommandHandler> logger)
    {
        _missingAlertRepository = Guard.Against.Null(missingAlertRepository);
        _petRepository = Guard.Against.Null(petRepository);
        _userRepository = Guard.Against.Null(userRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<MissingAlertResponse> Handle(CreateMissingAlertCommand request,
        CancellationToken cancellationToken)
    {
        Pet missingPet = await ValidateAndAssignPetAsync(request.PetId);

        CheckUserPermissionToCreate(missingPet.Owner.Id, request.UserId);

        User petOwner = await ValidateAndAssignUserAsync(request.UserId);

        Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
            request.LastSeenLocationLatitude,
            request.LastSeenLocationLongitude);

        MissingAlert missingAlertToCreate = new()
        {
            Id = _valueProvider.NewGuid(),
            RegistrationDate = _valueProvider.UtcNow(),
            Location = location,
            Description = request.Description,
            RecoveryDate = null,
            Pet = missingPet,
            User = petOwner
        };

        _missingAlertRepository.Add(missingAlertToCreate);
        await _missingAlertRepository.CommitAsync();

        return missingAlertToCreate.ToMissingAlertResponse();
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

    private void CheckUserPermissionToCreate(Guid userId, Guid requestUserId)
    {
        if (userId != requestUserId)
        {
            _logger.LogInformation(
                "Usuário {UserId} não possui permissão para criar alerta para usuário {RequestUserId}",
                userId, requestUserId);
            throw new ForbiddenException("Não é possível criar alertas para outros usuários.");
        }
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
}
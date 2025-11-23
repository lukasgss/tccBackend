using System.Diagnostics.CodeAnalysis;
using Application.Commands.Alerts.Common;
using Application.Commands.Pets.UploadImages;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.MissingAlerts.Create;

[ExcludeFromCodeCoverage]
public sealed record CreateMissingAlertCommand(
    int State,
    int City,
    string Neighborhood,
    string? Description,
    CreatePetRequest Pet,
    Guid UserId
) : IRequest<MissingAlertResponse>;

public sealed class CreateMissingAlertCommandHandler : IRequestHandler<CreateMissingAlertCommand, MissingAlertResponse>
{
    private readonly IMissingAlertRepository _missingAlertRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValueProvider _valueProvider;
    private readonly ILocationUtils _locationUtils;
    private readonly IBreedRepository _breedRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IColorRepository _colorRepository;
    private readonly ISender _mediator;
    private readonly ILogger<CreateMissingAlertCommandHandler> _logger;

    public CreateMissingAlertCommandHandler(
        IMissingAlertRepository missingAlertRepository,
        IPetRepository petRepository,
        IUserRepository userRepository,
        IValueProvider valueProvider,
        ILogger<CreateMissingAlertCommandHandler> logger,
        ILocationUtils locationUtils,
        IBreedRepository breedRepository,
        ISpeciesRepository speciesRepository,
        IColorRepository colorRepository,
        ISender mediator)
    {
        _locationUtils = locationUtils;
        _breedRepository = breedRepository;
        _speciesRepository = speciesRepository;
        _colorRepository = colorRepository;
        _mediator = mediator;
        _missingAlertRepository = Guard.Against.Null(missingAlertRepository);
        _userRepository = Guard.Against.Null(userRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<MissingAlertResponse> Handle(CreateMissingAlertCommand request,
        CancellationToken cancellationToken)
    {
        var alertOwner = await ValidateAndAssignUserAsync(request.UserId);

        Pet petToBeCreated = await GeneratePetToBeCreatedAsync(request.Pet, alertOwner, cancellationToken);

        AlertLocalization localizationData = await _locationUtils.GetAlertStateAndCity(request.State, request.City);
        Point location = await _locationUtils.GetAlertLocation(localizationData, request.Neighborhood);

        MissingAlert missingAlertToCreate = new()
        {
            Id = _valueProvider.NewGuid(),
            RegistrationDate = _valueProvider.UtcNow(),
            State = localizationData.State,
            City = localizationData.City,
            Neighborhood = request.Neighborhood,
            Location = location,
            Description = request.Description,
            RecoveryDate = null,
            Pet = petToBeCreated,
            User = alertOwner 
        };

        _missingAlertRepository.Add(missingAlertToCreate);
        await _missingAlertRepository.CommitAsync();

        return missingAlertToCreate.ToMissingAlertResponse();
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

    private async Task<Pet> GeneratePetToBeCreatedAsync(CreatePetRequest request, User user,
        CancellationToken cancellationToken = default)
    {
        Breed breed = await ValidateAndAssignBreedAsync(request.BreedId);
        Species species = await ValidateAndAssignSpeciesAsync(request.SpeciesId);
        List<Color> colors = await ValidateAndAssignColorsAsync(request.ColorIds);

        Guid petId = _valueProvider.NewGuid();

        if (request.Images.Count >= 10)
        {
            _logger.LogInformation("Não é possível adicionar {ImageCount} imagens", request.Images.Count);
            throw new BadRequestException("Não é possível adicionar 10 ou mais imagens");
        }

        Pet petToBeCreated = new()
        {
            Id = petId,
            Name = request.Name,
            Gender = request.Gender,
            Size = request.Size,
            Age = request.Age,
            IsVaccinated = request.IsVaccinated,
            IsCastrated = request.IsCastrated,
            IsNegativeToFivFelv = request.IsNegativeToFivFelv,
            IsNegativeToLeishmaniasis = request.IsNegativeToLeishmaniasis,
            Owner = user,
            UserId = user.Id,
            Breed = breed,
            BreedId = breed.Id,
            Species = species,
            SpeciesId = species.Id,
            Colors = colors,
            Images = []
        };

        UploadPetImagesCommand uploadImagesCommand = new(petToBeCreated, request.Images);
        List<PetImage> images = await _mediator.Send(uploadImagesCommand, cancellationToken);
        petToBeCreated.Images = images;

        return petToBeCreated;
    }

    private async Task<Breed> ValidateAndAssignBreedAsync(int breedId)
    {
        Breed? breed = await _breedRepository.GetBreedByIdAsync(breedId);
        if (breed is null)
        {
            _logger.LogInformation("Raça {BreedId} não existe", breedId);
            throw new NotFoundException("Raça especificada não existe.");
        }

        return breed;
    }

    private async Task<Species> ValidateAndAssignSpeciesAsync(int speciesId)
    {
        Species? species = await _speciesRepository.GetSpeciesByIdAsync(speciesId);
        if (species is null)
        {
            _logger.LogInformation("Espécie {SpeciesId} não existe", speciesId);
            throw new NotFoundException("Espécie especificada não existe.");
        }

        return species;
    }

    private async Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds)
    {
        List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
        if (colors.Count != colorIds.Count || colors.Count == 0)
        {
            _logger.LogInformation("Alguma das cores {@ColorIds} não existe", colorIds);
            throw new NotFoundException("Alguma das cores especificadas não existe.");
        }

        return colors;
    }
}
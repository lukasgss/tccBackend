using Application.Commands.Alerts.Common;
using Application.Commands.Pets.UploadImages;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.Providers;
using Ardalis.GuardClauses;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Events;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionAlerts.CreateAdoptionAlert;

public record CreateAdoptionAlertCommand(
    Guid UserId,
    List<string> AdoptionRestrictions,
    string Neighborhood,
    int State,
    int City,
    string? Description,
    CreatePetRequest Pet,
    bool ForceCreationWithNotFoundCoordinates,
    IFormFile? AdoptionForm,
    bool ShouldUseDefaultAdoptionForm) : IRequest<AdoptionAlertResponse>;

public class CreateAdoptionAlertCommandHandler : IRequestHandler<CreateAdoptionAlertCommand, AdoptionAlertResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IBreedRepository _breedRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IColorRepository _colorRepository;
    private readonly ISender _mediator;
    private readonly IValueProvider _valueProvider;
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly IAdoptionAlertFileSubmissionService _adoptionAlertFileSubmissionService;
    private readonly ILocationUtils _locationUtils;
    private readonly ILogger<CreateAdoptionAlertCommandHandler> _logger;

    public CreateAdoptionAlertCommandHandler(
        ISender mediator,
        IValueProvider valueProvider,
        IAdoptionAlertRepository adoptionAlertRepository,
        ILogger<CreateAdoptionAlertCommandHandler> logger,
        IUserRepository userRepository,
        ILocalizationRepository localizationRepository,
        IBreedRepository breedRepository,
        ISpeciesRepository speciesRepository,
        IColorRepository colorRepository,
        IAdoptionAlertFileSubmissionService adoptionAlertFileSubmissionService,
        ILocationUtils locationUtils)
    {
        _locationUtils = locationUtils;
        _adoptionAlertFileSubmissionService = Guard.Against.Null(adoptionAlertFileSubmissionService);
        _userRepository = Guard.Against.Null(userRepository);
        _breedRepository = Guard.Against.Null(breedRepository);
        _speciesRepository = Guard.Against.Null(speciesRepository);
        _colorRepository = Guard.Against.Null(colorRepository);
        _valueProvider = Guard.Against.Null(valueProvider);
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
        _logger = Guard.Against.Null(logger);
        _mediator = Guard.Against.Null(mediator);
    }

    public async Task<AdoptionAlertResponse> Handle(CreateAdoptionAlertCommand request,
        CancellationToken cancellationToken)
    {
        var alertOwner = await ValidateAndAssignUserAsync(request.UserId);

        AlertLocalization localizationData = await _locationUtils.GetAlertStateAndCity(request.State, request.City);
        Point? location = null;
        if (!request.ForceCreationWithNotFoundCoordinates)
        {
            location = await _locationUtils.GetAlertLocation(localizationData, request.Neighborhood);
        }

        Pet petToBeCreated = await GeneratePetToBeCreatedAsync(request.Pet, alertOwner);

        FileAttachment? adoptionForm = await GetFileAttachment(request, alertOwner);

        AdoptionAlert alertToBeCreated = new()
        {
            Id = _valueProvider.NewGuid(),
            AdoptionRestrictions = FormatAdoptionRestrictions(request.AdoptionRestrictions),
            Location = location,
            Description = request.Description,
            RegistrationDate = _valueProvider.UtcNow(),
            AdoptionDate = null,
            Neighborhood = request.Neighborhood,
            State = localizationData.State,
            City = localizationData.City,
            Pet = petToBeCreated,
            User = alertOwner,
            AdoptionForm = adoptionForm
        };

        UploadPetImagesCommand uploadImagesCommand = new(petToBeCreated, request.Pet.Images);
        List<PetImage> images = await _mediator.Send(uploadImagesCommand, cancellationToken);
        alertToBeCreated.Pet.Images = images;

        RaiseAlertCreatedEvent(alertToBeCreated);

        _adoptionAlertRepository.Add(alertToBeCreated);
        await _adoptionAlertRepository.CommitAsync();

        return alertToBeCreated.ToAdoptionAlertResponse();
    }

    private async Task<FileAttachment?> GetFileAttachment(CreateAdoptionAlertCommand request, User alertOwner)
    {
        string? adoptionFormUrl = await _adoptionAlertFileSubmissionService.UploadAdoptionFormAsync(
            request.AdoptionForm,
            previousAdoptionFormUrl: null,
            alertOwner.DefaultAdoptionFormUrl,
            request.ShouldUseDefaultAdoptionForm);

        FileAttachment? adoptionForm = null;
        if (adoptionFormUrl is not null && request.AdoptionForm is not null)
        {
            adoptionForm = new FileAttachment(adoptionFormUrl, request.AdoptionForm.FileName);
        }

        return adoptionForm;
    }

    private void RaiseAlertCreatedEvent(AdoptionAlert adoptionAlert)
    {
        var colorIds = adoptionAlert.Pet.Colors.Select(color => color.Id);
        AdoptionAlertCreated adoptionAlertCreatedEvent = new(
            adoptionAlert.Id,
            adoptionAlert.Pet.Gender,
            adoptionAlert.Pet.Age,
            adoptionAlert.Pet.Size,
            adoptionAlert.Location?.Y,
            adoptionAlert.Location?.X,
            adoptionAlert.Pet.SpeciesId,
            adoptionAlert.Pet.BreedId,
            colorIds,
            IsInSameTransaction: false,
            OwnerId: adoptionAlert.User.Id);

        adoptionAlert.AddDomainEvent(adoptionAlertCreatedEvent);
    }
    
    private async Task<User> ValidateAndAssignUserAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            _logger.LogInformation("Usuário {UserId} não existe", userId);
            throw new NotFoundException("Usuário com o id especificado não existe.");
        }

        return user;
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

    private async Task<Pet> GeneratePetToBeCreatedAsync(CreatePetRequest createPetRequest, User user)
    {
        Breed breed = await ValidateAndAssignBreedAsync(createPetRequest.BreedId);
        Species species = await ValidateAndAssignSpeciesAsync(createPetRequest.SpeciesId);
        List<Color> colors = await ValidateAndAssignColorsAsync(createPetRequest.ColorIds);

        Guid petId = _valueProvider.NewGuid();

        if (createPetRequest.Images.Count >= 10)
        {
            _logger.LogInformation("Não é possível adicionar {ImageCount} imagens", createPetRequest.Images.Count);
            throw new BadRequestException("Não é possível adicionar 10 ou mais imagens");
        }

        Pet petToBeCreated = new()
        {
            Id = petId,
            Name = createPetRequest.Name,
            Gender = createPetRequest.Gender,
            Size = createPetRequest.Size,
            Age = createPetRequest.Age,
            IsVaccinated = createPetRequest.IsVaccinated,
            IsCastrated = createPetRequest.IsCastrated,
            IsNegativeToFivFelv = createPetRequest.IsNegativeToFivFelv,
            IsNegativeToLeishmaniasis = createPetRequest.IsNegativeToLeishmaniasis,
            Owner = user,
            UserId = user.Id,
            Breed = breed,
            BreedId = breed.Id,
            Species = species,
            SpeciesId = species.Id,
            Colors = colors,
            Images = []
        };

        return petToBeCreated;
    }

    private static List<string> FormatAdoptionRestrictions(List<string> restrictions)
    {
        return restrictions.Select(restriction => restriction.Trim()
                .CapitalizeFirstLetter())
            .ToList();
    }
}
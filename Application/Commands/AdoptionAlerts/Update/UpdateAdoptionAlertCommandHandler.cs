using System.Diagnostics.CodeAnalysis;
using Application.Commands.Pets.UploadImages;
using Application.Common.Calculators;
using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.General.Location;
using Application.Common.Interfaces.Localization;
using Application.Queries.GeoLocation.GetCoordinatesFromStateAndCity;
using Ardalis.GuardClauses;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NotFoundException = Application.Common.Exceptions.NotFoundException;

namespace Application.Commands.AdoptionAlerts.Update;

[ExcludeFromCodeCoverage]
public sealed record UpdateAdoptionAlertCommand(
    Guid Id,
    string Neighborhood,
    int State,
    int City,
    string? Description,
    EditPetRequest Pet,
    Guid UserId,
    List<string> AdoptionRestrictions,
    bool ForceCreationWithNotFoundCoordinates,
    IFormFile? AdoptionForm,
    string? ExistingAdoptionFormUrl,
    bool ShouldUseDefaultAdoptionForm
) : IRequest<AdoptionAlertResponse>;

public sealed class UpdateAdoptionAlertCommandHandler
    : IRequestHandler<UpdateAdoptionAlertCommand, AdoptionAlertResponse>
{
    private readonly ISender _mediator;
    private readonly IAdoptionAlertRepository _adoptionAlertRepository;
    private readonly IPetImageRepository _petImageRepository;
    private readonly IPetImageSubmissionService _petImageSubmissionService;
    private readonly ILocalizationRepository _localizationRepository;
    private readonly IBreedRepository _breedRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IColorRepository _colorRepository;
    private readonly IAdoptionAlertFileSubmissionService _adoptionAlertFileSubmissionService;
    private readonly ILogger<UpdateAdoptionAlertCommandHandler> _logger;

    public UpdateAdoptionAlertCommandHandler(
        ISender mediator,
        IAdoptionAlertRepository adoptionAlertRepository,
        IPetImageRepository petImageRepository,
        IPetImageSubmissionService petImageSubmissionService,
        ILocalizationRepository localizationRepository,
        IBreedRepository breedRepository,
        ILogger<UpdateAdoptionAlertCommandHandler> logger,
        ISpeciesRepository speciesRepository,
        IColorRepository colorRepository,
        IAdoptionAlertFileSubmissionService adoptionAlertFileSubmissionService)
    {
        _adoptionAlertFileSubmissionService = Guard.Against.Null(adoptionAlertFileSubmissionService);
        _breedRepository = Guard.Against.Null(breedRepository);
        _speciesRepository = Guard.Against.Null(speciesRepository);
        _colorRepository = Guard.Against.Null(colorRepository);
        _petImageSubmissionService = Guard.Against.Null(petImageSubmissionService);
        _petImageRepository = Guard.Against.Null(petImageRepository);
        _adoptionAlertRepository = Guard.Against.Null(adoptionAlertRepository);
        _mediator = Guard.Against.Null(mediator);
        _localizationRepository = Guard.Against.Null(localizationRepository);
        _logger = Guard.Against.Null(logger);
    }

    public async Task<AdoptionAlertResponse> Handle(UpdateAdoptionAlertCommand request,
        CancellationToken cancellationToken)
    {
        var adoptionAlertDb = await ValidateAndAssignAdoptionAlertAsync(request.Id);
        ValidateIfUserIsOwnerOfAlert(adoptionAlertDb.User.Id, request.UserId);

        var localizationData = await GetAlertStateAndCity(request.State, request.City);
        Point? location = null;
        if (!request.ForceCreationWithNotFoundCoordinates)
        {
            location = await GetAlertLocation(localizationData, request.Neighborhood);
        }

        string? adoptionFormUrl = request.ExistingAdoptionFormUrl;
        if (string.IsNullOrEmpty(request.ExistingAdoptionFormUrl))
        {
            adoptionFormUrl = await _adoptionAlertFileSubmissionService.UploadAdoptionFormAsync(
                request.AdoptionForm,
                previousAdoptionFormUrl: adoptionAlertDb.AdoptionForm?.FileUrl,
                adoptionAlertDb.User.DefaultAdoptionFormUrl,
                request.ShouldUseDefaultAdoptionForm);
        }

        adoptionAlertDb.AdoptionRestrictions = FormatAdoptionRestrictions(request.AdoptionRestrictions);
        adoptionAlertDb.Neighborhood = request.Neighborhood;
        adoptionAlertDb.Location = location;

        FileAttachment? adoptionForm = null;
        if (adoptionFormUrl is not null && request.AdoptionForm?.Name is not null)
        {
            adoptionForm = new FileAttachment(adoptionFormUrl, request.AdoptionForm.FileName);
        }

        adoptionAlertDb.AdoptionForm = adoptionForm;

        adoptionAlertDb.Description = request.Description;

        await UpdatePet(adoptionAlertDb.Pet, request.Pet);

        await _adoptionAlertRepository.CommitAsync();

        return adoptionAlertDb.ToAdoptionAlertResponse();
    }

    private async Task<Point?> GetAlertLocation(AlertLocalization localizationData, string neighborhood)
    {
        GetCoordinatesFromStateAndCityQuery query = new(
            neighborhood,
            localizationData.State.Name,
            localizationData.City.Name);
        var addressData = await _mediator.Send(query);

        if (addressData is null)
        {
            throw new NotFoundException("Não foi possível encontrar coordenadas do bairro especificado.");
        }

        return CoordinatesCalculator.CreatePointBasedOnCoordinates(
            double.Parse(addressData.Latitude),
            double.Parse(addressData.Longitude));
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

    private async Task UpdatePet(Pet existingPet, EditPetRequest editedPet)
    {
        existingPet.Name = editedPet.Name;
        existingPet.Gender = editedPet.Gender;
        existingPet.Age = editedPet.Age;
        existingPet.Size = editedPet.Size;
        existingPet.IsVaccinated = editedPet.IsVaccinated;
        existingPet.IsNegativeToFivFelv = editedPet.IsNegativeToFivFelv;
        existingPet.IsNegativeToLeishmaniasis = editedPet.IsNegativeToLeishmaniasis;
        existingPet.IsCastrated = editedPet.IsCastrated;

        Breed breed = await ValidateAndAssignBreedAsync(editedPet.BreedId);
        existingPet.Breed = breed;

        Species species = await ValidateAndAssignSpeciesAsync(editedPet.SpeciesId);
        existingPet.Species = species;

        List<PetImage> currentPetImages = await UpdatePetImages(editedPet.ExistingImages, existingPet.Id);
        IList<PetImage> newlyAddedImages =
            await _mediator.Send(new UploadPetImagesCommand(existingPet, editedPet.Images ?? []));
        List<PetImage> combinedImages = [..currentPetImages, ..newlyAddedImages];
        existingPet.Images = combinedImages;

        var colors = await ValidateAndAssignColorsAsync(editedPet.ColorIds);
        existingPet.Colors = colors;
    }

    private async Task<List<PetImage>> UpdatePetImages(IEnumerable<string> sentExistingImages, Guid petId)
    {
        var petImages = await _petImageRepository.GetImagesFromPetByIdAsync(petId);

        var differentPetImages = petImages.Where(image => !sentExistingImages.Contains(image.ImageUrl))
            .ToList();
        if (differentPetImages.Count > 0)
        {
            await _petImageSubmissionService.DeletePetImageAsync(petId, differentPetImages);
        }

        var maintainedPetImages = petImages.Where(image => sentExistingImages.Contains(image.ImageUrl))
            .ToList();
        return maintainedPetImages;
    }

    private async Task<AlertLocalization> GetAlertStateAndCity(int stateId, int cityId)
    {
        State state = await GetState(stateId);
        City city = await GetCity(cityId);

        return new AlertLocalization()
        {
            State = state,
            City = city
        };
    }

    private async Task<State> GetState(int stateId)
    {
        var state = await _localizationRepository.GetStateById(stateId);
        if (state is null)
        {
            _logger.LogInformation("Estado de id {EstadoId} não foi encontrado.", stateId);
            throw new NotFoundException("Estado especificado não foi encontrado.");
        }

        return state;
    }

    private async Task<City> GetCity(int cityId)
    {
        var city = await _localizationRepository.GetCityById(cityId);
        if (city is null)
        {
            _logger.LogInformation("Cidade de id {CidadeId} não foi encontrada.", cityId);
            throw new NotFoundException("Cidade especificada não foi encontrada.");
        }

        return city;
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

    private static List<string> FormatAdoptionRestrictions(List<string> restrictions)
    {
        return restrictions.Select(restriction => restriction.Trim()
                .CapitalizeFirstLetter())
            .ToList();
    }
}
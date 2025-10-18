using Application.Common.Calculators;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;
using Domain.Enums;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Commands.FoundAlertPreferences;

public record AssignFoundAlertUserPreferencesCommand(
    Guid UserId,
    double? FoundLocationLatitude,
    double? FoundLocationLongitude,
    double? RadiusDistanceInKm,
    List<Gender>? Genders,
    List<Age>? Ages,
    List<Size>? Sizes,
    List<int>? SpeciesIds,
    List<int>? BreedIds,
    List<int>? ColorIds
) : IRequest<UserPreferencesResponse>;

public class AssignFoundAlertUserPreferencesCommandHandler
    : IRequestHandler<AssignFoundAlertUserPreferencesCommand, UserPreferencesResponse>
{
    private readonly IFoundAnimalUserPreferencesRepository _foundAnimalUserPreferencesRepository;
    private readonly IUserPreferencesValidations _userPreferencesValidations;
    private readonly IValueProvider _valueProvider;

    public AssignFoundAlertUserPreferencesCommandHandler(
        IFoundAnimalUserPreferencesRepository foundAnimalUserPreferencesRepository,
        IUserPreferencesValidations userPreferencesValidations, IValueProvider valueProvider)
    {
        _foundAnimalUserPreferencesRepository = foundAnimalUserPreferencesRepository;
        _userPreferencesValidations = userPreferencesValidations;
        _valueProvider = valueProvider;
    }

    public async Task<UserPreferencesResponse> Handle(AssignFoundAlertUserPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        FoundAnimalUserPreferences? dbPreferences =
            await _foundAnimalUserPreferencesRepository.GetUserPreferences(request.UserId);

        var species =
            await _userPreferencesValidations.ValidateAndAssignSpeciesAsync(request.SpeciesIds);
        var breeds =
            await _userPreferencesValidations.ValidateAndAssignBreedAsync(request.BreedIds, species);
        var colors = await _userPreferencesValidations.ValidateAndAssignColorsAsync(request.ColorIds);
        User user = await _userPreferencesValidations.AssignUserAsync(request.UserId);

        Point? location = null;
        if (request.FoundLocationLatitude is not null &&
            request.FoundLocationLongitude is not null &&
            request.RadiusDistanceInKm is not null)
        {
            location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
                request.FoundLocationLatitude.Value,
                request.FoundLocationLongitude!.Value);
        }

        if (dbPreferences is not null)
        {
            dbPreferences.User = user;
            dbPreferences.UserId = user.Id;
            dbPreferences.Colors = colors;
            dbPreferences.Breeds = breeds;
            dbPreferences.Species = species;
            dbPreferences.Sizes = request.Sizes;
            dbPreferences.Ages = request.Ages;
            dbPreferences.Genders = request.Genders;
            dbPreferences.Location = location;
            dbPreferences.RadiusDistanceInKm = request.RadiusDistanceInKm;

            _foundAnimalUserPreferencesRepository.Update(dbPreferences);
            await _foundAnimalUserPreferencesRepository.CommitAsync();

            return dbPreferences.ToFoundAnimalUserPreferencesResponse();
        }

        FoundAnimalUserPreferences foundAnimalUserPreferences = new()
        {
            Id = _valueProvider.NewGuid(),
            User = user,
            UserId = user.Id,
            Colors = colors,
            Breeds = breeds,
            Species = species,
            Sizes = request.Sizes,
            Ages = request.Ages,
            Genders = request.Genders,
            Location = location,
            RadiusDistanceInKm = request.RadiusDistanceInKm
        };

        _foundAnimalUserPreferencesRepository.Add(foundAnimalUserPreferences);

        await _foundAnimalUserPreferencesRepository.CommitAsync();
        return foundAnimalUserPreferences.ToFoundAnimalUserPreferencesResponse();
    }
}
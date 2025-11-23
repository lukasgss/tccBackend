using System.Diagnostics.CodeAnalysis;
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

namespace Application.Commands.AdoptionAlertPreferences.Assign;

[ExcludeFromCodeCoverage]
public record AssignAdoptionUserPreferencesCommand(
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

public class AssignAdoptionUserPreferencesCommandHandler :
    IRequestHandler<AssignAdoptionUserPreferencesCommand, UserPreferencesResponse>
{
    private readonly IAdoptionUserPreferencesRepository _adoptionUserPreferencesRepository;
    private readonly IUserPreferencesValidations _userPreferencesValidations;
    private readonly IValueProvider _valueProvider;

    public AssignAdoptionUserPreferencesCommandHandler(
        IAdoptionUserPreferencesRepository adoptionUserPreferencesRepository,
        IUserPreferencesValidations userPreferencesValidations,
        IValueProvider valueProvider)
    {
        _adoptionUserPreferencesRepository = adoptionUserPreferencesRepository;
        _userPreferencesValidations = userPreferencesValidations;
        _valueProvider = valueProvider;
    }

    public async Task<UserPreferencesResponse> Handle(AssignAdoptionUserPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        AdoptionUserPreferences? dbPreferences =
            await _adoptionUserPreferencesRepository.GetUserPreferences(request.UserId);

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
            dbPreferences.Colors = colors;
            dbPreferences.Ages = request.Ages;
            dbPreferences.Breeds = breeds;
            dbPreferences.Species = species;
            dbPreferences.Sizes = request.Sizes;
            dbPreferences.Genders = request.Genders;
            dbPreferences.Location = location;
            dbPreferences.RadiusDistanceInKm = request.RadiusDistanceInKm;

            _adoptionUserPreferencesRepository.Update(dbPreferences);
            await _adoptionUserPreferencesRepository.CommitAsync();

            return dbPreferences.ToAdoptionUserPreferencesResponse();
        }

        AdoptionUserPreferences adoptionUserPreferences = new()
        {
            Id = _valueProvider.NewGuid(),
            User = user,
            Colors = colors,
            Ages = request.Ages,
            Breeds = breeds,
            Species = species,
            Sizes = request.Sizes,
            Genders = request.Genders,
            Location = location,
            RadiusDistanceInKm = request.RadiusDistanceInKm
        };

        _adoptionUserPreferencesRepository.Add(adoptionUserPreferences);
        await _adoptionUserPreferencesRepository.CommitAsync();

        return adoptionUserPreferences.ToAdoptionUserPreferencesResponse();
    }
}
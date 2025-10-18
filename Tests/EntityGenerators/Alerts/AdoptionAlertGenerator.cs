using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Application.Commands.AdoptionAlerts.CreateAdoptionAlert;
using Application.Commands.AdoptionAlerts.Update;
using Application.Common.Calculators;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Queries.AdoptionFavorites.GetById;
using Application.Queries.GeoLocation.Common;
using Domain.Entities.Alerts;
using Domain.Enums;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts;

public static class AdoptionAlertGenerator
{
    public static AdoptionAlert GenerateAdoptedAdoptionAlert()
    {
        return new AdoptionAlert()
        {
            Id = Constants.AdoptionAlertData.Id,
            AdoptionRestrictions = Constants.AdoptionAlertData.AdoptionRestrictions,
            Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
                Constants.AdoptionAlertData.LocationLatitude,
                Constants.AdoptionAlertData.LocationLongitude),
            Neighborhood = Constants.AdoptionAlertData.Address,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = Constants.AdoptionAlertData.AdoptedAdoptionDate,
            Pet = Constants.AdoptionAlertData.Pet,
            PetId = Constants.AdoptionAlertData.PetId,
            User = Constants.AdoptionAlertData.User,
            UserId = Constants.AdoptionAlertData.UserId,
            City = Constants.LocationData.City,
            State = Constants.LocationData.State
        };
    }

    public static AdoptionAlert GenerateNonAdoptedAdoptionAlert()
    {
        return new AdoptionAlert()
        {
            Id = Constants.AdoptionAlertData.Id,
            AdoptionRestrictions = Constants.AdoptionAlertData.AdoptionRestrictions,
            Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
                Constants.AdoptionAlertData.LocationLatitude,
                Constants.AdoptionAlertData.LocationLongitude),
            Neighborhood = Constants.AdoptionAlertData.Address,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = null,
            Pet = Constants.AdoptionAlertData.Pet,
            PetId = Constants.AdoptionAlertData.PetId,
            User = Constants.AdoptionAlertData.User,
            UserId = Constants.AdoptionAlertData.UserId,
            City = Constants.LocationData.City,
            State = Constants.LocationData.State
        };
    }

    public static AdoptionAlert GenerateAdoptionAlertWithRandomOwner()
    {
        return new AdoptionAlert()
        {
            Id = Constants.AdoptionAlertData.Id,
            AdoptionRestrictions = Constants.AdoptionAlertData.AdoptionRestrictions,
            Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
                Constants.AdoptionAlertData.LocationLatitude,
                Constants.AdoptionAlertData.LocationLongitude),
            Neighborhood = Constants.AdoptionAlertData.Address,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = null,
            Pet = Constants.AdoptionAlertData.Pet,
            PetId = Constants.AdoptionAlertData.PetId,
            User = UserGenerator.GenerateUserWithRandomId(),
            UserId = UserGenerator.GenerateUserWithRandomId().Id,
            City = Constants.LocationData.City,
            State = Constants.LocationData.State
        };
    }

    public static AdoptionAlertListing GenerateAdoptionAlertListing()
    {
        return new AdoptionAlertListing()
        {
            Id = Constants.AdoptionAlertData.Id,
            AdoptionRestrictions = Constants.AdoptionAlertData.AdoptionRestrictions,
            Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
                Constants.AdoptionAlertData.LocationLatitude,
                Constants.AdoptionAlertData.LocationLongitude),
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = null,
            Pet = Constants.AdoptionAlertData.Pet,
            User = UserGenerator.GenerateUserWithRandomId(),
            IsFavorite = false
        };
    }

    public static List<AdoptionAlertListing> GenerateListOfAdoptionAlertListings()
    {
        List<AdoptionAlertListing> listings = new(3);
        for (int i = 0; i < 3; i++)
        {
            listings.Add(GenerateAdoptionAlertListing());
        }

        return listings;
    }

    public static List<AdoptionAlertListingResponse> GenerateListOfAdoptionAlertListingResponse()
    {
        return GenerateListOfAdoptionAlertListings()
            .Select(listing => listing.ToAdoptionAlertListingResponse())
            .ToList();
    }

    public static CreateAdoptionAlertRequest GenerateCreateAdoptionAlertRequest()
    {
        return new CreateAdoptionAlertRequest(
            AdoptionRestrictions: Constants.AdoptionAlertData.AdoptionRestrictions,
            Neighborhood: Constants.AdoptionAlertData.Neighborhood,
            State: Constants.AdoptionAlertData.StateId,
            City: Constants.AdoptionAlertData.CityId,
            Description: Constants.AdoptionAlertData.Description,
            Pet: PetGenerator.GenerateCreatePetRequest()
        );
    }

    public static UpdateAdoptionAlertRequest GenerateEditAdoptionAlertRequest()
    {
        return new UpdateAdoptionAlertRequest(
            Neighborhood: Constants.AdoptionAlertData.Neighborhood,
            State: Constants.AdoptionAlertData.StateId,
            City: Constants.AdoptionAlertData.CityId,
            AdoptionRestrictions: Constants.AdoptionAlertData.AdoptionRestrictions,
            Description: Constants.AdoptionAlertData.Description,
            Pet: PetGenerator.GenerateEditPetRequest()
        );
    }

    public static AdoptionAlertResponse GenerateAdoptedAdoptionAlertResponse()
    {
        return new AdoptionAlertResponse(
            Id: Constants.AdoptionAlertData.Id,
            AdoptionRestrictions: Constants.AdoptionAlertData.AdoptionRestrictions,
            LocationLatitude: Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude: Constants.AdoptionAlertData.LocationLongitude,
            Neighborhood: Constants.AdoptionAlertData.Neighborhood,
            Description: Constants.AdoptionAlertData.Description,
            RegistrationDate: Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate: Constants.AdoptionAlertData.AdoptedAdoptionDate,
            Pet: Constants.AdoptionAlertData.Pet.ToPetResponseNoOwner(),
            Owner: Constants.AdoptionAlertData.User.ToUserDataResponse()
        );
    }

    public static AdoptionAlertResponse GenerateNonAdoptedAdoptionAlertResponse()
    {
        return new AdoptionAlertResponse(
            Id: Constants.AdoptionAlertData.Id,
            AdoptionRestrictions: Constants.AdoptionAlertData.AdoptionRestrictions,
            LocationLatitude: Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude: Constants.AdoptionAlertData.LocationLongitude,
            Neighborhood: Constants.AdoptionAlertData.Neighborhood,
            Description: Constants.AdoptionAlertData.Description,
            RegistrationDate: Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate: null,
            Pet: Constants.AdoptionAlertData.Pet.ToPetResponseNoOwner(),
            Owner: Constants.AdoptionAlertData.User.ToUserDataResponse()
        );
    }

    public static GeoLocationResponse GenerateGeoLocationResponse()
    {
        return new GeoLocationResponse(
            Latitude: Constants.AdoptionAlertData.LocationLatitude.ToString(CultureInfo.InvariantCulture),
            Longitude: Constants.AdoptionAlertData.LocationLongitude.ToString(CultureInfo.InvariantCulture),
            Address: Constants.AdoptionAlertData.Address,
            PostalCode: Constants.AdoptionAlertData.PostalCode,
            State: Constants.AdoptionAlertData.State,
            City: Constants.AdoptionAlertData.City,
            Neighborhood: Constants.AdoptionAlertData.Neighborhood
        );
    }

    public static AdoptionAlertResponseWithGeoLocation GenerateAdoptionAlertResponseWithGeoLocation()
    {
        return new AdoptionAlertResponseWithGeoLocation(
            Id: Constants.AdoptionAlertData.Id,
            AdoptionRestrictions: Constants.AdoptionAlertData.AdoptionRestrictions,
            LocationLatitude: Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude: Constants.AdoptionAlertData.LocationLongitude,
            Neighborhood: Constants.AdoptionAlertData.Neighborhood,
            Description: Constants.AdoptionAlertData.Description,
            StateId: Constants.AdoptionAlertData.StateId,
            CityId: Constants.AdoptionAlertData.CityId,
            RegistrationDate: Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate: null,
            IsFavorite: false,
            Pet: Constants.AdoptionAlertData.Pet.ToPetResponseNoOwner(),
            Owner: Constants.AdoptionAlertData.User.ToUserDataResponse(),
            GeoLocation: AlertGeoLocationGenerator.GenerateGeoLocation()
        );
    }

    public static List<AdoptionAlertResponse> GenerateListOfAlertsResponse()
    {
        List<AdoptionAlertResponse> adoptionAlerts = new(3);
        for (var i = 0; i < 3; i++) adoptionAlerts.Add(GenerateAdoptedAdoptionAlertResponse());

        return adoptionAlerts;
    }

    public static List<AdoptionAlert> GenerateListOfAlerts()
    {
        List<AdoptionAlert> adoptionAlerts = new(3);
        for (int i = 0; i < 3; i++)
        {
            adoptionAlerts.Add(GenerateAdoptedAdoptionAlert());
        }

        return adoptionAlerts;
    }

    public static AdoptionAlertFilters GenerateAdotionAlertFilters()
    {
        return new AdoptionAlertFilters()
        {
            Latitude = 29.977329046788345,
            Longitude = 31.132637435581703,
            RadiusDistanceInKm = 5,
            Adopted = false,
            NotAdopted = true,
            SpeciesId = 1,
            BreedIds = [1],
            ColorIds = [1],
            GenderIds = [Gender.Macho],
            AgeIds = [Age.Adulto],
            SizeIds = [Size.Grande]
        };
    }
}
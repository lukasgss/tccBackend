using Application.Common.DTOs;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Queries.AdoptionAlerts.GetById;
using Application.Queries.AdoptionFavorites.GetById;
using Domain.Common;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class AdoptionAlertMappings
{
    public static AdoptionAlertResponse ToAdoptionAlertResponse(this AdoptionAlert adoptionAlert)
    {
        return new AdoptionAlertResponse(
            Id: adoptionAlert.Id,
            AdoptionRestrictions: adoptionAlert.AdoptionRestrictions,
            LocationLatitude: adoptionAlert.Location?.Y,
            LocationLongitude: adoptionAlert.Location?.X,
            Neighborhood: adoptionAlert.Neighborhood,
            Description: adoptionAlert.Description,
            AdoptionForm: adoptionAlert.AdoptionForm is not null
                ? new FileAttachment(adoptionAlert.AdoptionForm.FileUrl,
                    adoptionAlert.AdoptionForm.FileName)
                : null,
            RegistrationDate: adoptionAlert.RegistrationDate,
            AdoptionDate: adoptionAlert.AdoptionDate,
            Pet: adoptionAlert.Pet.ToPetResponseNoOwner(),
            Owner: adoptionAlert.User.ToUserDataResponse()
        );
    }

    public static AdoptionAlertResponseWithGeoLocation ToAdoptionAlertResponseWithGeoLocation(
        this AdoptionAlertByIdQueryResponse adoptionAlert, AlertGeoLocation? geoLocation, bool isFavorite)
    {
        return new AdoptionAlertResponseWithGeoLocation(
            Id: adoptionAlert.Id,
            AdoptionRestrictions: adoptionAlert.AdoptionRestrictions,
            LocationLatitude: adoptionAlert.Location?.Y,
            LocationLongitude: adoptionAlert.Location?.X,
            Description: adoptionAlert.Description,
            RegistrationDate: adoptionAlert.RegistrationDate,
            AdoptionDate: adoptionAlert.AdoptionDate,
            IsFavorite: isFavorite,
            Neighborhood: adoptionAlert.Neighborhood,
            StateId: adoptionAlert.State.Id,
            CityId: adoptionAlert.City.Id,
            Pet: adoptionAlert.Pet,
            Owner: adoptionAlert.User,
            GeoLocation: geoLocation,
            AdoptionForm: adoptionAlert.AdoptionForm is not null
                ? new FileAttachment(adoptionAlert.AdoptionForm.FileUrl,
                    adoptionAlert.AdoptionForm.FileName)
                : null
        );
    }

    public static SimplifiedAdoptionAlertResponse ToSimplifiedAdoptionAlertResponse(this AdoptionAlert adoptionAlert)
    {
        return new SimplifiedAdoptionAlertResponse(
            Id: adoptionAlert.Id,
            AdoptionRestrictions: adoptionAlert.AdoptionRestrictions,
            LocationLatitude: adoptionAlert.Location?.Y,
            LocationLongitude: adoptionAlert.Location?.X,
            Description: adoptionAlert.Description,
            RegistrationDate: adoptionAlert.RegistrationDate,
            AdoptionDate: adoptionAlert.AdoptionDate,
            Pet: adoptionAlert.Pet.ToSimplifiedPetResponse()
        );
    }

    public static AdoptionAlertListingResponse ToAdoptionAlertListingResponse(
        this AdoptionAlertListing adoptionAlertListing)
    {
        return new AdoptionAlertListingResponse(
            Id: adoptionAlertListing.Id,
            AdoptionRestrictions: adoptionAlertListing.AdoptionRestrictions,
            LocationLatitude: adoptionAlertListing.Location?.Y,
            LocationLongitude: adoptionAlertListing.Location?.X,
            Description: adoptionAlertListing.Description,
            RegistrationDate: adoptionAlertListing.RegistrationDate,
            AdoptionDate: adoptionAlertListing.AdoptionDate,
            Pet: adoptionAlertListing.Pet.ToExtraSimplifiedPetResponse(),
            Owner: adoptionAlertListing.User.ToUserDataResponse(),
            IsFavorite: adoptionAlertListing.IsFavorite
        );
    }

    public static List<AdoptionAlertListingResponse> ToAdoptionAlertListingResponse(
        this IEnumerable<AdoptionAlertListing> adoptionAlertListings)
    {
        return adoptionAlertListings.Select(alert => alert.ToAdoptionAlertListingResponse())
            .ToList();
    }
}
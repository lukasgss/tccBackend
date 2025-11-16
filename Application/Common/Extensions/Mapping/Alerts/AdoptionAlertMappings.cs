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
    extension(AdoptionAlert adoptionAlert)
    {
        public AdoptionAlertResponse ToAdoptionAlertResponse()
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

        public SimplifiedAdoptionAlertResponse ToSimplifiedAdoptionAlertResponse()
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
    }

    extension(AdoptionAlertByIdQueryResponse adoptionAlert)
    {
        public AdoptionAlertResponseWithGeoLocation ToAdoptionAlertResponseWithGeoLocation(
            AlertGeoLocation? geoLocation, bool isFavorite)
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
    }

    extension(AdoptionAlertListing adoptionAlertListing)
    {
        public AdoptionAlertListingResponse ToAdoptionAlertListingResponse()
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
    }

    extension(IEnumerable<AdoptionAlertListing> adoptionAlertListings)
    {
        public List<AdoptionAlertListingResponse> ToAdoptionAlertListingResponse()
        {
            return adoptionAlertListings.Select(alert => alert.ToAdoptionAlertListingResponse())
                .ToList();
        }
    }
}
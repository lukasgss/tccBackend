using Application.Common.DTOs;
using Application.Common.GeoLocation;
using Application.Queries.MissingAlerts.GetById;
using Application.Queries.MissingAlerts.ListMissingAlerts;
using Domain.Entities.Alerts;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class MissingAlertMappings
{
    extension(MissingAlert missingAlert)
    {
        public MissingAlertResponse ToMissingAlertResponse()
        {
            return new MissingAlertResponse(
                Id: missingAlert.Id,
                RegistrationDate: missingAlert.RegistrationDate,
                State: missingAlert.State,
                City: missingAlert.City,
                Neighborhood: missingAlert.Neighborhood,
                Description: missingAlert.Description,
                RecoveryDate: missingAlert.RecoveryDate,
                Pet: missingAlert.Pet.ToPetResponseNoOwner(),
                Owner: missingAlert.User.ToOwnerResponse()
            );
        }
    }

    extension(MissingAlertQueryResponse missingAlert)
    {
        public MissingAlertResponse ToMissingAlertResponseFromQuery()
        {
            return new MissingAlertResponse(
                Id: missingAlert.Id,
                RegistrationDate: missingAlert.RegistrationDate,
                State: missingAlert.State,
                City: missingAlert.City,
                Neighborhood: missingAlert.Neighborhood,
                Description: missingAlert.Description,
                RecoveryDate: missingAlert.RecoveryDate,
                Pet: missingAlert.Pet,
                Owner: missingAlert.Owner
            );
        }
    }

    extension(MissingAlertByIdQueryResponse missingAlert)
    {
        public MissingAlertResponseWithGeoLocation ToMissingAlertResponseWithGeoLocation(AlertGeoLocation location)
        {
            return new MissingAlertResponseWithGeoLocation(
                Id: missingAlert.Id,
                RegistrationDate: missingAlert.RegistrationDate,
                Description: missingAlert.Description,
                LastSeenLocationLatitude: missingAlert.LastSeenLocationLatitude,
                LastSeenLocationLongitude: missingAlert.LastSeenLocationLongitude,
                RecoveryDate: missingAlert.RecoveryDate,
                Pet: missingAlert.Pet,
                Owner: missingAlert.Owner,
                GeoLocation: location
            );
        }
    }
}
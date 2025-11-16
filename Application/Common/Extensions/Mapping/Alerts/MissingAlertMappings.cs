using Application.Common.DTOs;
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
                LastSeenLocationLatitude: missingAlert.Location.Y,
                LastSeenLocationLongitude: missingAlert.Location.X,
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
                LastSeenLocationLatitude: missingAlert.Location.Y,
                LastSeenLocationLongitude: missingAlert.Location.X,
                Description: missingAlert.Description,
                RecoveryDate: missingAlert.RecoveryDate,
                Pet: missingAlert.Pet,
                Owner: missingAlert.Owner
            );
        }
    }
}
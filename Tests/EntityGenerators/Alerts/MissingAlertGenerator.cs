using System.Collections.Generic;
using Application.Commands.MissingAlerts.Create;
using Application.Commands.MissingAlerts.Edit;
using Application.Common.Calculators;
using Application.Common.DTOs;
using Application.Common.Extensions.Mapping;
using Application.Queries.MissingAlerts.ListMissingAlerts;
using Domain.Entities.Alerts;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.EntityGenerators.Alerts;

public static class MissingAlertGenerator
{
    public static MissingAlert GenerateMissingAlert()
    {
        return new MissingAlert()
        {
            Id = Constants.MissingAlertData.Id,
            RegistrationDate = Constants.MissingAlertData.RegistrationDate,
            Location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
                Constants.MissingAlertData.LastSeenLocationLatitude,
                Constants.MissingAlertData.LastSeenLocationLongitude),
            RecoveryDate = Constants.MissingAlertData.NonRecoveredRecoveryDate,
            Description = Constants.MissingAlertData.Description,
            Pet = Constants.MissingAlertData.Pet,
            PetId = Constants.MissingAlertData.PetId,
            User = Constants.MissingAlertData.User,
            UserId = Constants.MissingAlertData.UserId
        };
    }

    public static List<MissingAlert> GenerateListOfAlerts()
    {
        List<MissingAlert> missingAlerts = new();
        for (int i = 0; i < 3; i++)
        {
            missingAlerts.Add(GenerateMissingAlert());
        }

        return missingAlerts;
    }

    public static MissingAlertResponse GenerateMissingAlertResponse()
    {
        return new MissingAlertResponse()
        {
            Id = Constants.MissingAlertData.Id,
            RegistrationDate = Constants.MissingAlertData.RegistrationDate,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            Description = Constants.MissingAlertData.Description,
            RecoveryDate = Constants.MissingAlertData.NonRecoveredRecoveryDate,
            Pet = Constants.MissingAlertData.Pet.ToPetResponseNoOwner(),
            Owner = Constants.MissingAlertData.User.ToOwnerResponse()
        };
    }

    public static List<MissingAlertResponse> GenerateListOfAlertsResponse()
    {
        List<MissingAlertResponse> alertResponses = new();
        for (int i = 0; i < 3; i++)
        {
            alertResponses.Add(GenerateMissingAlertResponse());
        }

        return alertResponses;
    }

    public static MissingAlertResponse GenerateRecoveredMissingAlertResponse()
    {
        return new MissingAlertResponse()
        {
            Id = Constants.MissingAlertData.Id,
            RegistrationDate = Constants.MissingAlertData.RegistrationDate,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            Description = Constants.MissingAlertData.Description,
            RecoveryDate = Constants.MissingAlertData.RecoveryDate,
            Pet = Constants.MissingAlertData.Pet.ToPetResponseNoOwner(),
            Owner = Constants.MissingAlertData.User.ToOwnerResponse()
        };
    }

    public static CreateMissingAlertRequest GenerateCreateMissingAlert()
    {
        return new CreateMissingAlertRequest()
        {
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            Description = Constants.MissingAlertData.Description,
            PetId = Constants.PetData.Id
        };
    }

    public static EditMissingAlertRequest GenerateEditMissingAlertRequest()
    {
        return new EditMissingAlertRequest()
        {
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            Description = Constants.MissingAlertData.Description,
            PetId = Constants.MissingAlertData.PetId
        };
    }

    public static MissingAlertFilters GenerateMissingAlertFilters()
    {
        return new MissingAlertFilters()
        {
            Latitude = 29.977329046788345,
            Longitude = 31.132637435581703,
            RadiusDistanceInKm = 5,
            Missing = true,
            NotMissing = false
        };
    }
}
﻿using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Domain.Entities.Alerts;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class FoundAnimalAlertMappings
{
    public static FoundAnimalAlertResponse ToFoundAnimalAlertResponse(this FoundAnimalAlert foundAnimalAlert)
    {
        return new FoundAnimalAlertResponse(
            Id: foundAnimalAlert.Id,
            Name: foundAnimalAlert.Name,
            Description: foundAnimalAlert.Description,
            FoundLocationLatitude: foundAnimalAlert.Location.Y,
            FoundLocationLongitude: foundAnimalAlert.Location.X,
            RegistrationDate: foundAnimalAlert.RegistrationDate,
            RecoveryDate: foundAnimalAlert.RecoveryDate,
            Age: Enum.GetName(typeof(Age), foundAnimalAlert.Age)!,
            Size: Enum.GetName(typeof(Size), foundAnimalAlert.Size)!,
            Images: foundAnimalAlert.Images.ToFoundAlertImagesResponse(),
            Species: foundAnimalAlert.Species.ToSpeciesResponse(),
            Breed: foundAnimalAlert.Breed?.ToBreedResponse(),
            Owner: foundAnimalAlert.User.ToUserDataResponse(),
            Gender: foundAnimalAlert.Gender is null ? null : Enum.GetName(typeof(Gender), foundAnimalAlert.Gender),
            Colors: foundAnimalAlert.Colors.ToListOfColorResponse()
        );
    }

    private static List<string> ToFoundAlertImagesResponse(this IEnumerable<FoundAnimalAlertImage> images)
    {
        return images.Select(alertImage => alertImage.ImageUrl).ToList();
    }
}
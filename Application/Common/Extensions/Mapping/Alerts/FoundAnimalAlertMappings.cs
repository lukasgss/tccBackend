using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities.Alerts;
using Domain.ValueObjects;

namespace Application.Common.Extensions.Mapping.Alerts;

public static class FoundAnimalAlertMappings
{
    extension(FoundAnimalAlert foundAnimalAlert)
    {
        public FoundAnimalAlertResponse ToFoundAnimalAlertResponse()
        {
            return new FoundAnimalAlertResponse(
                Id: foundAnimalAlert.Id,
                Name: foundAnimalAlert.Name,
                Description: foundAnimalAlert.Description,
                FoundLocationLatitude: foundAnimalAlert.Location.Y,
                FoundLocationLongitude: foundAnimalAlert.Location.X,
                RegistrationDate: foundAnimalAlert.RegistrationDate,
                RecoveryDate: foundAnimalAlert.RecoveryDate,
                Pet: new ExtraSimplifiedPetResponse(Id: foundAnimalAlert.Id,
                    Age: foundAnimalAlert.Age.ToAgeResponse(),
                    Name: foundAnimalAlert.Name!, Breed: foundAnimalAlert.Breed!.ToBreedResponse(),
                    Gender: foundAnimalAlert.Gender.ToGenderResponse()!,
                    Images: foundAnimalAlert.Images.Select(x => x.ImageUrl).ToList()),
                Owner: new UserDataResponse(Id: foundAnimalAlert.Id, Image: foundAnimalAlert.User.Image,
                    FullName: foundAnimalAlert.User.FullName, Email: foundAnimalAlert.User.Email!,
                    PhoneNumber: foundAnimalAlert.User.PhoneNumber,
                    OnlyWhatsAppMessages: foundAnimalAlert.User.ReceivesOnlyWhatsAppMessages,
                    DefaultAdoptionFormUrl: foundAnimalAlert.User.DefaultAdoptionFormUrl)
            );
        }

        public FoundAnimalAlertResponseWithGeoLocation ToFoundAnimalAlertResponseWithGeoLocation(
            AlertGeoLocation formattedLocation)
        {
            return new FoundAnimalAlertResponseWithGeoLocation(
                Id: foundAnimalAlert.Id,
                Name: foundAnimalAlert.Name,
                Description: foundAnimalAlert.Description,
                FoundLocationLatitude: foundAnimalAlert.Location.Y,
                FoundLocationLongitude: foundAnimalAlert.Location.X,
                RegistrationDate: foundAnimalAlert.RegistrationDate,
                RecoveryDate: foundAnimalAlert.RecoveryDate,
                Pet: new ExtraSimplifiedPetResponse(
                    Id: foundAnimalAlert.Id,
                    Age: foundAnimalAlert.Age.ToAgeResponse(),
                    Name: foundAnimalAlert.Name!, Breed: foundAnimalAlert.Breed!.ToBreedResponse(),
                    Gender: foundAnimalAlert.Gender.ToGenderResponse()!,
                    Images: foundAnimalAlert.Images.Select(x => x.ImageUrl).ToList()),
                Owner: new UserDataResponse(Id: foundAnimalAlert.User.Id, Image: foundAnimalAlert.User.Image,
                    FullName: foundAnimalAlert.User.FullName, Email: foundAnimalAlert.User.Email!,
                    PhoneNumber: foundAnimalAlert.User.PhoneNumber,
                    OnlyWhatsAppMessages: foundAnimalAlert.User.ReceivesOnlyWhatsAppMessages,
                    DefaultAdoptionFormUrl: foundAnimalAlert.User.DefaultAdoptionFormUrl),
                GeoLocation: formattedLocation
            );
        }

        public SimplifiedFoundAlertResponse ToSimplifiedFoundAlertResponse()
        {
            return new SimplifiedFoundAlertResponse(
                Id: foundAnimalAlert.Id,
                LocationLatitude: foundAnimalAlert.Location?.Y,
                LocationLongitude: foundAnimalAlert.Location?.X,
                Description: foundAnimalAlert.Description,
                RegistrationDate: foundAnimalAlert.RegistrationDate,
                Pet: new SimplifiedPetResponse(Id: foundAnimalAlert.Id, Name: foundAnimalAlert.Name!,
                    Gender: foundAnimalAlert.Gender.ToGenderResponse()!.Name,
                    Age: foundAnimalAlert.Age.ToAgeResponse().Name,
                    Size: foundAnimalAlert.Size.ToSizeResponse().Name,
                    Images: foundAnimalAlert.Images.Select(x => x.ImageUrl).ToList())
            );
        }
    }

    private static List<string> ToFoundAlertImagesResponse(this IEnumerable<FoundAnimalAlertImage> images)
    {
        return images.Select(alertImage => alertImage.ImageUrl).ToList();
    }
}
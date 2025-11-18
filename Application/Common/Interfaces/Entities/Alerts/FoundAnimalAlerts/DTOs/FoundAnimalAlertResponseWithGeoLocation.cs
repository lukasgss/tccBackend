using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public sealed record FoundAnimalAlertResponseWithGeoLocation(
	Guid Id,
	string? Name,
	string? Description,
	double FoundLocationLatitude,
	double FoundLocationLongitude,
	DateTime RegistrationDate,
	DateOnly? RecoveryDate,
	ExtraSimplifiedPetResponse Pet,
	UserDataResponse Owner,
	AlertGeoLocation? GeoLocation
);
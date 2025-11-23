using System.Diagnostics.CodeAnalysis;
using Application.Common.DTOs;
using Application.Common.GeoLocation;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;

namespace Application.Queries.MissingAlerts.GetById;

[ExcludeFromCodeCoverage]
public sealed record MissingAlertResponseWithGeoLocation(
	Guid Id,
	DateTime RegistrationDate,
	double LastSeenLocationLatitude,
	double LastSeenLocationLongitude,
	string? Description,
	DateOnly? RecoveryDate,
	PetResponseNoOwner Pet,
	AlertUserDataResponse Owner,
	AlertGeoLocation GeoLocation
);
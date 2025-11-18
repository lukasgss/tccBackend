using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public sealed record SimplifiedFoundAlertResponse(
	Guid Id,
	double? LocationLatitude,
	double? LocationLongitude,
	string? Description,
	DateTime RegistrationDate,
	SimplifiedPetResponse Pet
);
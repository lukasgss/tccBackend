using Application.Common.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities;

namespace Application.Queries.MissingAlerts.GetById;

public sealed record MissingAlertByIdQueryResponse(
	Guid Id,
	DateTime RegistrationDate,
	string? Description,
	DateOnly? RecoveryDate,
	double LastSeenLocationLatitude,
	double LastSeenLocationLongitude,
	City City,
	State State,
	string Neighborhood,
	PetResponseNoOwner Pet,
	AlertUserDataResponse Owner);
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities;

namespace Application.Queries.AdoptionAlerts.GetUserCreatedAlerts;

public sealed class CreatedAdoptionListingResponse
{
	public Guid Id { get; init; }

	public string? Description { get; init; }

	public DateTime RegistrationDate { get; init; }

	public DateOnly? AdoptionDate { get; init; }

	public List<string> AdoptionRestrictions { get; init; } = [];

	public ExtraSimplifiedPetResponse Pet { get; init; } = null!;

	public City City { get; init; } = null!;
	public UserDataResponse Owner { get; init; } = null!;
	public bool IsFavorite { get; init; }
}

public sealed record CreatedAlertsResponse(
	List<CreatedAdoptionListingResponse> AdoptionAlerts,
	List<FoundAnimalAlertResponse> FoundAnimalAlerts);
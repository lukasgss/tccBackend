using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Queries.Users.Common;
using Domain.Entities;

namespace Application.Queries.AdoptionAlerts.GetUserSavedAlerts;

[ExcludeFromCodeCoverage]
public sealed class SavedAdoptionListingResponse
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
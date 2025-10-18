using Domain.Entities;
using NetTopologySuite.Geometries;

namespace Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

public class AdoptionAlertListing
{
    public Guid Id { get; init; }

    public Point? Location { get; init; }

    public string? Description { get; init; }

    public DateTime RegistrationDate { get; init; }

    public DateOnly? AdoptionDate { get; init; }

    public List<string> AdoptionRestrictions { get; init; } = [];

    public Pet Pet { get; init; } = null!;

    public City City { get; init; } = null!;

    public User User { get; init; } = null!;
    public bool IsFavorite { get; init; }
}
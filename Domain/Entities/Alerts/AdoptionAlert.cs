using Domain.Common;
using Domain.Entities.Alerts.Notifications;
using Domain.Entities.Alerts.UserFavorites;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Alerts;

public class AdoptionAlert : BaseEntity
{
    public Guid Id { get; set; }
    public Point? Location { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
    public FileAttachment? AdoptionForm { get; set; }
    public DateOnly? AdoptionDate { get; set; }
    public List<string> AdoptionRestrictions { get; set; } = [];

    public Pet Pet { get; set; } = null!;
    public Guid PetId { get; set; }

    public User User { get; set; } = null!;
    public Guid UserId { get; set; }

    public State State { get; set; } = null!;
    public City City { get; set; } = null!;
    public required string Neighborhood { get; set; } = null!;
    public IList<AdoptionFavorite> AdoptionFavorites { get; set; } = [];
    public IList<AdoptionAlertNotification> AlertNotifications { get; set; } = [];

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
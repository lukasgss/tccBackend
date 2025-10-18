using Domain.Entities.Alerts;
using Domain.Entities.Alerts.Notifications;
using Domain.Entities.Alerts.UserFavorites;
using Domain.Entities.Alerts.UserPreferences;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public string Image { get; set; } = null!;
    public bool ReceivesOnlyWhatsAppMessages { get; set; }
    public string? DefaultAdoptionFormUrl { get; set; }
    public FoundAnimalUserPreferences? FoundAnimalUserPreferences { get; set; }
    public AdoptionUserPreferences? AdoptionUserPreferences { get; set; }
    public IList<AdoptionAlert> AdoptionAlerts { get; set; } = [];
    public IList<FoundAnimalAlertNotifications> FoundAnimalAlertNotifications { get; set; } = null!;
    public IList<AdoptionFavorite> AdoptionFavorites { get; set; } = null!;
    public IList<AdoptionReport> AdoptionReports { get; set; } = [];
    public IList<AdoptionAlertNotification> AdoptionAlertNotifications { get; set; } = [];
}
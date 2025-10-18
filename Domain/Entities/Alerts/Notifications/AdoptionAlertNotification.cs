namespace Domain.Entities.Alerts.Notifications;

public class AdoptionAlertNotification
{
    public long Id { get; set; }
    public DateTime TimeStampUtc { get; set; }
    public IList<User> Users { get; set; } = [];
    public AdoptionAlert AdoptionAlert { get; set; } = null!;
    public bool HasBeenRead { get; set; }
}
namespace Application.Queries.AdoptionAlertNotifications.GetNotifications;

public record AdoptionAlertNotificationResponse(
    long Id,
    Guid AlertId,
    string AdoptionImageUrl,
    bool HasBeenRead,
    DateTime CreatedAt);
using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.AdoptionAlertNotifications.GetNotifications;

[ExcludeFromCodeCoverage]
public sealed record AdoptionAlertNotificationResponse(
    long Id,
    Guid AlertId,
    string AdoptionImageUrl,
    bool HasBeenRead,
    DateTime CreatedAt);
namespace Application.Commands.MissingAlerts.Create;

public record CreateMissingAlertRequest(
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    Guid PetId
);
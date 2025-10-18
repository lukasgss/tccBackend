namespace Application.Commands.MissingAlerts.Edit;

public record EditMissingAlertRequest(
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    Guid PetId
);
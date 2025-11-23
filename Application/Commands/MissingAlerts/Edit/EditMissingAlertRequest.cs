using System.Diagnostics.CodeAnalysis;

namespace Application.Commands.MissingAlerts.Edit;

[ExcludeFromCodeCoverage]
public sealed record EditMissingAlertRequest(
    double LastSeenLocationLatitude,
    double LastSeenLocationLongitude,
    string? Description,
    Guid PetId
);
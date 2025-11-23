using System.Diagnostics.CodeAnalysis;

namespace Application.Events.AdoptionAlerts.CreatedAdoptionAlert;

[ExcludeFromCodeCoverage]
public sealed record UserThatMatchPreferences(Guid UserId);
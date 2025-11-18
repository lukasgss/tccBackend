using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

namespace Application.Common.DTOs;

public sealed record FoundAnimalFavoriteResponse(Guid Id, SimplifiedFoundAlertResponse FoundAlert);
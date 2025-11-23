using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record FoundAnimalFavoriteResponse(Guid Id, SimplifiedFoundAlertResponse FoundAlert);
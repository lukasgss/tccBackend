using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record AdoptionFavoriteResponse(Guid Id, SimplifiedAdoptionAlertResponse AdoptionAlert);
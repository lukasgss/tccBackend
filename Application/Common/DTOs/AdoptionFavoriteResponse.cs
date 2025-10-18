using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

namespace Application.Common.DTOs;

public record AdoptionFavoriteResponse(Guid Id, SimplifiedAdoptionAlertResponse AdoptionAlert);
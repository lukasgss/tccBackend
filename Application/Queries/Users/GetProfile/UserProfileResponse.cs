using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

namespace Application.Queries.Users.GetProfile;

public record UserProfileResponse(
    Guid Id,
    string Image,
    string FullName,
    string Email,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages,
    List<AdoptionAlertProfileListing> AdoptionAlerts
);
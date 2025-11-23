using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;

namespace Application.Queries.Users.GetProfile;

[ExcludeFromCodeCoverage]
public record UserProfileResponse(
    Guid Id,
    string Image,
    string FullName,
    string Email,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages,
    List<AdoptionAlertProfileListing> AdoptionAlerts
);
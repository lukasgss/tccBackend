using System.Diagnostics.CodeAnalysis;

namespace Application.Common.DTOs;

[ExcludeFromCodeCoverage]
public sealed record UserResponse(
    Guid Id,
    string Email,
    string FullName,
    string Image,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages,
    string AccessToken,
    string RefreshToken
);
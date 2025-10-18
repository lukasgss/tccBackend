namespace Application.Common.DTOs;

public record UserResponse(
    Guid Id,
    string Email,
    string FullName,
    string Image,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages,
    string AccessToken,
    string RefreshToken
);
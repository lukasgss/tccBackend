namespace Application.Queries.Users.Common;

public record UserDataResponse(
    Guid Id,
    string Image,
    string FullName,
    string Email,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages,
    string? DefaultAdoptionFormUrl
);
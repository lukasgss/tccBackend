namespace Application.Queries.Users.Common;

public record AlertUserDataResponse(
    Guid Id,
    string Image,
    string FullName,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages
);
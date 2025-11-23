using System.Diagnostics.CodeAnalysis;

namespace Application.Queries.Users.Common;

[ExcludeFromCodeCoverage]
public record AlertUserDataResponse(
    Guid Id,
    string Image,
    string FullName,
    string? PhoneNumber,
    bool OnlyWhatsAppMessages
);